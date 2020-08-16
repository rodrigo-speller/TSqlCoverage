// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

namespace TSqlCoverage.Metadata
{
    public abstract class SqlServerObjectMetadata
    {
        public SqlServerObjectMetadata(SqlServerSchemaMetadata schema, int objectId, SqlIdentifier name)
        {
            Schema = schema ?? throw new System.ArgumentNullException(nameof(schema));
            Name = name ?? throw new System.ArgumentNullException(nameof(name));

            ObjectId = objectId;
        }

        public SqlServerSchemaMetadata Schema { get; }
        public int ObjectId { get; }
        public SqlIdentifier Name { get; }

        public override string ToString()
        {
            var schema = Schema;

            return $"{schema.Database.Name.QuotedIdentifier}.{schema.Name.QuotedIdentifier}.{Name.QuotedIdentifier}";
        }
    }
}
