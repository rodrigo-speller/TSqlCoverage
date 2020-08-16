// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

namespace TSqlCoverage.Metadata
{
    public abstract class SqlServerModuleObjectMetadata : SqlServerObjectMetadata
    {
        public SqlServerModuleObjectMetadata(SqlServerSchemaMetadata Schema, int objectId, SqlIdentifier name, SqlServerModuleMetadata module)
            : base(Schema, objectId, name)
        {
            Module = module;
        }

        public SqlServerModuleMetadata Module { get; internal set; }
    }
}
