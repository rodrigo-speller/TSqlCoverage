// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.SqlParser.Common;

namespace TSqlCoverage.Metadata
{
    public sealed class SqlServerMetadataProvider
    {
        private readonly SqlConnection connection;

        private readonly SqlCommandBuilder commandBuilder = new SqlCommandBuilder();

        private SemaphoreSlim readLock = new SemaphoreSlim(1, 1);

        private readonly ConcurrentDictionary<int, Task<SqlServerDatabaseMetadata>> databases
            = new ConcurrentDictionary<int, Task<SqlServerDatabaseMetadata>>();
        private readonly ConcurrentDictionary<int, Task<SqlServerSchemaMetadata>> schemas
            = new ConcurrentDictionary<int, Task<SqlServerSchemaMetadata>>();
        private readonly ConcurrentDictionary<(int databaseId, int objectId), Task<SqlServerStoredProcedureMetadata>> procedures
            = new ConcurrentDictionary<(int databaseId, int objectId), Task<SqlServerStoredProcedureMetadata>>();
        private readonly ConcurrentDictionary<(int databaseId, int objectId), Task<SqlServerFunctionMetadata>> functions
            = new ConcurrentDictionary<(int databaseId, int objectId), Task<SqlServerFunctionMetadata>>();

        public SqlServerMetadataProvider(SqlConnection connection)
        {
            this.connection = connection
                ?? throw new System.ArgumentNullException(nameof(connection));
        }

        public async Task<SqlServerDatabaseMetadata> GetDatabaseMetadataById(int databaseId)
        {
            return await databases.GetOrAdd(
                databaseId,
                async _ => {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            SELECT name, compatibility_level
                                FROM master.sys.databases
                                WHERE database_id = @database_id
                        ";

                        command.Parameters.Add(new SqlParameter("@database_id", SqlDbType.Int) {
                            Value = databaseId
                        });

                        string name;
                        byte compatibility_level;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                                return null;

                            name = reader.GetString(0);
                            compatibility_level = reader.GetByte(1);
                        }

                        var compatibilityLevelKey = $"Sql{compatibility_level.ToString(CultureInfo.InvariantCulture)}";

                        if (!Enum.TryParse<DatabaseCompatibilityLevel>(compatibilityLevelKey, out var compatibilityLevel))
                            compatibilityLevel = DatabaseCompatibilityLevel.Current;

                        return new SqlServerDatabaseMetadata(
                            databaseId,
                            new SqlIdentifier(name, false),
                            compatibilityLevel
                        );
                    }
                }
            );
        }

        public async Task<SqlServerFunctionMetadata> GetFunctionMetadataById(int databaseId, int objectId, bool definition)
        {
            var fn = await functions.GetOrAdd(
                (databaseId, objectId),
                async _ => {
                    var database = await GetDatabaseMetadataById(databaseId);

                    if (database is null)
                        return null;
 
                    var databaseIdentifier = database.Name.DelimitedIdentifier;

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $@"
                            SELECT name, schema_id
                                FROM {databaseIdentifier}.sys.objects
                                WHERE object_id = @object_id
                                    AND type IN (
                                        'AF', 'FN', 'FS', 'FT', 'IF', 'TF'
                                    )
                        ";

                        command.Parameters.Add(new SqlParameter("@object_id", SqlDbType.Int) {
                            Value = objectId
                        });

                        string name;
                        int schemaId;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                                return null;

                            name = reader.GetString(0);
                            schemaId = reader.GetInt32(1);
                        }
                        
                        var schema = await GetSchemaMetadataById(databaseId, schemaId);

                        return new SqlServerFunctionMetadata(schema, objectId, new SqlIdentifier(name, false), null);
                    }
                }
            );

            if (fn is null)
                return null;

            if (definition)
                await EnsureModule(fn);

            return fn;
        }

        public async Task<SqlServerSchemaMetadata> GetSchemaMetadataById(int databaseId, int schemaId)
        {
            return await schemas.GetOrAdd(
                databaseId,
                async _ => {
                    var database = await GetDatabaseMetadataById(databaseId);

                    if (database is null)
                        return null;

                    var databaseIdentifier = database.Name.DelimitedIdentifier;

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $@"
                            SELECT name
                                FROM {databaseIdentifier}.sys.schemas
                                WHERE schema_id = @schema_id
                        ";

                        command.Parameters.Add(new SqlParameter("@schema_id", SqlDbType.Int) {
                            Value = schemaId
                        });

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                                return null;

                            var name = reader.GetString(0);

                            return new SqlServerSchemaMetadata(database, schemaId, new SqlIdentifier(name, false));
                        }
                    }
                }
            );
        }

        public async Task<SqlServerStoredProcedureMetadata> GetStoredProcedureMetadataById(int databaseId, int objectId, bool definition)
        {
            var sp = await procedures.GetOrAdd(
                (databaseId, objectId),
                async _ => {
                    var database = await GetDatabaseMetadataById(databaseId);

                    if (database is null)
                        return null;

                    var databaseIdentifier = database.Name.DelimitedIdentifier;

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $@"
                            SELECT name, schema_id
                                FROM {databaseIdentifier}.sys.procedures
                                WHERE object_id = @object_id
                        ";

                        command.Parameters.Add(new SqlParameter("@object_id", SqlDbType.Int) {
                            Value = objectId
                        });

                        string name;
                        int schemaId;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                                return null;

                            name = reader.GetString(0);
                            schemaId = reader.GetInt32(1);
                        }
                        
                        var schema = await GetSchemaMetadataById(databaseId, schemaId);

                        return new SqlServerStoredProcedureMetadata(
                            schema,
                            objectId,
                            new SqlIdentifier(name, false),
                            null
                        );
                    }
                }
            );

            if (sp is null)
                return null;

            if (definition)
                await EnsureModule(sp);

            return sp;
        }

        private async Task EnsureModule(SqlServerModuleObjectMetadata sp)
        {
            if (!(sp.Module is null))
                return;

            readLock.Wait();

            try
            {
                if (sp.Module is null)
                    sp.Module = await LoadModule(sp);
            }
            finally
            {
                readLock.Release();
            }
        }

        private async Task<SqlServerModuleMetadata> LoadModule(SqlServerModuleObjectMetadata obj)
        {
            using (var command = connection.CreateCommand())
            {
                var objectId = obj.ObjectId;
                var databaseIdentifier = obj.Schema.Database.Name.DelimitedIdentifier;

                command.CommandText = $@"
                    SELECT definition, uses_quoted_identifier
                        FROM {databaseIdentifier}.sys.sql_modules
                        WHERE object_id = @object_id
                ";

                command.Parameters.Add(new SqlParameter("@object_id", SqlDbType.Int) {
                    Value = objectId
                });

                string definition;
                bool usesQuotedIdentifier;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    definition = reader.GetString(0);
                    usesQuotedIdentifier = reader.GetBoolean(1);
                }

                return new SqlServerModuleMetadata(obj, definition, usesQuotedIdentifier);
            }
        }
    }
}
