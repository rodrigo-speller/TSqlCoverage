// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using Microsoft.SqlServer.Management.SqlParser.Common;
using Microsoft.SqlServer.Management.SqlParser.Parser;

namespace TSqlCoverage.Metadata
{
    public class SqlServerDatabaseMetadata
    {
        public SqlServerDatabaseMetadata(int databaseId, SqlIdentifier name, DatabaseCompatibilityLevel compatibilityLevel)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            CompatibilityLevel = compatibilityLevel;
            DatabaseId = databaseId;
        }

        public int DatabaseId { get; }
        public SqlIdentifier Name { get; }
        public DatabaseCompatibilityLevel CompatibilityLevel { get; }

        public override string ToString()
            => Name.QuotedIdentifier;
    }
}
