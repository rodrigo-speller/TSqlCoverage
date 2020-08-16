// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;

namespace TSqlCoverage.Metadata
{
    public sealed class SqlServerSchemaMetadata
    {
        public SqlServerSchemaMetadata(SqlServerDatabaseMetadata database, int schemaId, SqlIdentifier name)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            SchemaId = schemaId;
        }

        public SqlServerDatabaseMetadata Database { get; }
        public int SchemaId { get; }
        public SqlIdentifier Name { get; }

        public override string ToString()
            => $"{Database.Name.QuotedIdentifier}.{Name.QuotedIdentifier}";
    }
}
