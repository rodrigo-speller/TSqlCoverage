// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;

namespace TSqlCoverage.Metadata
{
    public class SqlServerModuleMetadata
    {
        public SqlServerModuleMetadata(
            SqlServerModuleObjectMetadata moduleObject,
            string definition,
            bool usesQuotedIdentifier)
        {
            ModuleObject = moduleObject ?? throw new ArgumentNullException(nameof(moduleObject));
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            UsesQuotedIdentifier = usesQuotedIdentifier;
        }

        public SqlServerModuleObjectMetadata ModuleObject { get; }
        public string Definition { get; }
        public bool UsesQuotedIdentifier { get; }

        public SqlStatement Parse()
        {
            var database = ModuleObject.Schema.Database;
            
            ParseResult result;

            try
            {
                result = Parser.Parse(Definition, new ParseOptions() {
                    CompatibilityLevel = database.CompatibilityLevel,
                    IsQuotedIdentifierSet = UsesQuotedIdentifier
                });
            }
            catch (Exception ex)
            {
                throw new TSqlParseException(ex);
            }

            var errors = result.Errors?.ToArray();
            if (errors.Length > 0)
                throw new TSqlParseException(errors);

            var script = result.Script;
            if (script.Batches.Count != 1)
                throw new TSqlParseException("The object module definition must contains only one batch");

            var batch = script.Batches[0];
            if (batch.Statements.Count != 1)
                throw new TSqlParseException("The object module definition must contains only one statement");

            var statement = batch.Statements[0];

            return statement;
        }
    }
}
