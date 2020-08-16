// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

namespace TSqlCoverage.Metadata
{
    public sealed class SqlServerStoredProcedureMetadata : SqlServerModuleObjectMetadata
    {
        public SqlServerStoredProcedureMetadata(SqlServerSchemaMetadata schema, int objectId, SqlIdentifier name, SqlServerModuleMetadata module)
            : base (schema, objectId, name, module)
        { }
    }
}
