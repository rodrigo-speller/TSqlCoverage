// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.IO;
using TSqlCoverage.XEvents.Internal;
using Xunit;

namespace TSqlCoverage.Core.UnitTests.XEvents.Predicates
{
    public class XEventUnicodeStringValueTests
    {
        [Fact]
        public void Create_With_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new XEventUnicodeStringValue(null));
        }

        [Fact]
        public void Create_Implicit_Cast()
        {
            XEventUnicodeStringValue xeValue = "some_value";

            using (var writer = new StringWriter())
            {
                xeValue.WriteTo(writer);
                Assert.Equal("N'some_value'", writer.ToString());
            }
        }

        [Theory]
        [InlineData("", @"N''")]
        [InlineData("'", @"N''''")]
        [InlineData("''", @"N''''''")]
        [InlineData("'some_value", @"N'''some_value'")]
        [InlineData("some'value", @"N'some''value'")]
        [InlineData("some_value'", @"N'some_value'''")]
        [InlineData("'some'value'", @"N'''some''value'''")]
        [InlineData("''some''value''", @"N'''''some''''value'''''")]
        public void Encoding_Values(string value, string sqlValue)
        {
            var xeValue = new XEventUnicodeStringValue(value);
            
            using (var writer = new StringWriter())
            {
                xeValue.WriteTo(writer);
                Assert.Equal(sqlValue, writer.ToString());
            }
        }
    }
}
