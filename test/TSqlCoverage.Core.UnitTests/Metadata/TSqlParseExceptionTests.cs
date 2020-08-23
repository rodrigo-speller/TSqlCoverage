// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using TSqlCoverage.Metadata;
using Xunit;

namespace TSqlCoverage.Core.UnitTests.Metadata
{
    public class TSqlParseExceptionTests
    {
        [Fact]
        public void No_Parameters_Constructor()
        {
            try
            {
                throw new TSqlParseException();
            }
            catch (TSqlParseException exception)
            {
                Assert.Contains("T-SQL code", exception.Message);

                Assert.NotNull(exception.Errors);
                Assert.Empty(exception.Errors);
            }
        }

        [Fact]
        public void InnerException_Constructor()
        {
            try
            {
                throw new TSqlParseException(new Exception());
            }
            catch (TSqlParseException exception)
            {
                Assert.Contains("T-SQL code", exception.Message);

                Assert.NotNull(exception.Errors);
                Assert.Empty(exception.Errors);
            }
        }

        [Fact]
        public void CustomMessage_Constructor()
        {
            try
            {
                throw new TSqlParseException("some custom message");
            }
            catch (TSqlParseException exception)
            {
                Assert.Contains("some custom message", exception.Message);

                Assert.NotNull(exception.Errors);
                Assert.Empty(exception.Errors);
            }
        }

        [Fact]
        public void NullMessage_Constructor()
        {
            try
            {
                throw new TSqlParseException((string)null);
            }
            catch (TSqlParseException exception)
            {
                Assert.Contains("T-SQL code", exception.Message);

                Assert.NotNull(exception.Errors);
                Assert.Empty(exception.Errors);
            }
        }

        [Fact]
        public void NullErrors_Constructor()
        {
            Assert.Throws<ArgumentNullException>(() => new TSqlParseException((IEnumerable<Error>)null));
        }

        [Fact]
        public void Errors_Serialization()
        {
            var parseResult = Parser.Parse(@"
                THIS IS AN INVALID T-SQL STATEMENT;
                GO;
                THIS IS ANOTHER INVALID T-SQL STATEMENT;
                GO;
            ");

            if (!parseResult.Errors.Any())
                throw new InvalidOperationException();

            var source = new TSqlParseException(parseResult.Errors.ToArray());

            TSqlParseException target;
            var binaryFormatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                binaryFormatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                target = (TSqlParseException)binaryFormatter.Deserialize(stream);
            }

            Assert.NotNull(target);
            Assert.Equal(source.Message, target.Message);

            Assert.NotNull(target.Errors);
            Assert.Equal(source.Errors.Count, target.Errors.Count);
            Assert.All(source.Errors.Zip(target.Errors), x => {
                Assert.Equal(x.First.Message, x.Second.Message);
                Assert.Equal(x.First.IsWarning, x.Second.IsWarning);
                Assert.Equal(x.First.Type, x.Second.Type);
                Assert.Equal(x.First.Start, x.Second.Start);
                Assert.Equal(x.First.End, x.Second.End);
            });
        }
    }
}
