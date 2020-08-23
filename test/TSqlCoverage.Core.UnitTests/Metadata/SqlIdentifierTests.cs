// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.SqlServer.Management.SqlParser.Metadata;
using TSqlCoverage.Metadata;
using Xunit;

namespace TSqlCoverage.Core.UnitTests.Metadata
{
    public class SqlIdentifierTests
    {
        public static object[][] Latin1_CS_AS_Identifiers()
            => new []
            {
                new [] { "[identifier]", "identifier" },
                new [] { "[identifier with spaces]", "identifier with spaces" },
                new [] { "[[escaped identifier]]]", "[escaped identifier]" }
            };

        public static object[][] Latin1_CI_AS_Identifiers()
            => new []
            {
                new [] { "[iDenTifier]", "IdEntiFier" },
                new [] { "[IdeNtifier With Spaces]", "identifier WITH spaces" },
                new [] { "[[Escaped Identifier]]]", "[ESCAPED IDENTIFIER]" }
            };

        public static object[][] Latin1_CI_AI_Identifiers()
            => new []
            {
                new [] { "[íDênTifier]", "IdEntiFíër" },
                new [] { "[IdeNtifier Wìth Spaces]", "identifier WITH spaces" },
                new [] { "[[Escaped Idêntifier]]]", "[ESCAPED IDENTIFIER]" }
            };

        private static (SqlIdentifier Identifier, int HashCode)[] HashCodeData(Func<SqlIdentifier, int> hasher)
        {
            var identifiers = Latin1_CI_AI_Identifiers()
                .Concat(Latin1_CI_AS_Identifiers())
                .Concat(Latin1_CS_AS_Identifiers())
                .SelectMany(x => new [] {
                    new SqlIdentifier((string)x[0], true),
                    new SqlIdentifier((string)x[1], false)
                })
                .Select(x => (x, hasher(x)))
                .ToArray();

            return identifiers;
        }

        [Theory]
        [MemberData(nameof(Latin1_CS_AS_Identifiers))]
        public void RegularIdentifier_Constructor(string delimited, string regular)
        {
            var identifier = new SqlIdentifier(regular, false);

            Assert.Equal(regular, identifier.Identifier);
            Assert.Equal(delimited, identifier.DelimitedIdentifier);
        }

        [Fact]
        public void RegularIdentifier_Constructor_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SqlIdentifier(null, false));
        }

        [Theory]
        [MemberData(nameof(Latin1_CS_AS_Identifiers))]
        public void DelimitedIdentifier_Constructor(string delimited, string regular)
        {
            var identifier = new SqlIdentifier(delimited, true);

            Assert.Equal(regular, identifier.Identifier);
            Assert.Equal(delimited, identifier.DelimitedIdentifier);
        }

        [Fact]
        public void DelimitedIdentifier_Constructor_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SqlIdentifier(null, true));
        }

        [Theory]
        [MemberData(nameof(Latin1_CS_AS_Identifiers))]
        [MemberData(nameof(Latin1_CI_AS_Identifiers))]
        public void SameReference_Equals(string delimited, string regular)
        {
            var regularIdentifier = new SqlIdentifier(regular, false);
            var delimitedIdentifier = new SqlIdentifier(delimited, true);

            Assert.True(regularIdentifier.Equals(regularIdentifier));
            Assert.True(regularIdentifier.Equals((object)regularIdentifier));
            Assert.True(delimitedIdentifier.Equals(delimitedIdentifier));
            Assert.True(delimitedIdentifier.Equals((object)delimitedIdentifier));
        }

        [Theory]
        [MemberData(nameof(Latin1_CS_AS_Identifiers))]
        [MemberData(nameof(Latin1_CI_AS_Identifiers))]
        public void Default_CaseInsensitive_Equals(string delimited, string regular)
        {
            var regularIdentifier = new SqlIdentifier(regular, false);
            var delimitedIdentifier = new SqlIdentifier(delimited, true);

            Assert.True(regularIdentifier.Equals((object)delimitedIdentifier));
            Assert.True(regularIdentifier.Equals((object)delimitedIdentifier.Identifier));
            Assert.True(regularIdentifier.Equals(delimitedIdentifier.Identifier));
            Assert.True(delimitedIdentifier.Equals((object)regularIdentifier));
            Assert.True(delimitedIdentifier.Equals((object)regularIdentifier.Identifier));
            Assert.True(delimitedIdentifier.Equals(regularIdentifier.Identifier));
        }

        [Theory]
        [MemberData(nameof(Latin1_CI_AI_Identifiers))]
        public void Default_CaseInsensitive_Equals_WithAccents(string delimited, string regular)
        {
            var regularIdentifier = new SqlIdentifier(regular, false);
            var delimitedIdentifier = new SqlIdentifier(delimited, true);

            Assert.False(regularIdentifier.Equals((object)delimitedIdentifier));
            Assert.False(regularIdentifier.Equals((object)delimitedIdentifier.Identifier));
            Assert.False(regularIdentifier.Equals(delimitedIdentifier.Identifier));
            Assert.False(delimitedIdentifier.Equals((object)regularIdentifier));
            Assert.False(delimitedIdentifier.Equals((object)regularIdentifier.Identifier));
            Assert.False(delimitedIdentifier.Equals(regularIdentifier.Identifier));
        }

        [Theory]
        [MemberData(nameof(Latin1_CI_AS_Identifiers))]
        [MemberData(nameof(Latin1_CS_AS_Identifiers))]
        public void Ordinal_CaseInsensitive_Equals(string delimited, string regular)
        {
            var regularIdentifier = new SqlIdentifier(regular, false);
            var delimitedIdentifier = new SqlIdentifier(delimited, true);

            Assert.True(regularIdentifier.Equals((object)delimitedIdentifier, CollationInfo.OrdinalIgnoreCase));
            Assert.True(regularIdentifier.Equals((object)delimitedIdentifier.Identifier, CollationInfo.OrdinalIgnoreCase));
            Assert.True(regularIdentifier.Equals(delimitedIdentifier.Identifier, CollationInfo.OrdinalIgnoreCase));
            Assert.True(delimitedIdentifier.Equals((object)regularIdentifier, CollationInfo.OrdinalIgnoreCase));
            Assert.True(delimitedIdentifier.Equals((object)regularIdentifier.Identifier, CollationInfo.OrdinalIgnoreCase));
            Assert.True(delimitedIdentifier.Equals(regularIdentifier.Identifier, CollationInfo.OrdinalIgnoreCase));
        }

        [Theory]
        [MemberData(nameof(Latin1_CI_AI_Identifiers))]
        public void Ordinal_CaseInsensitive_Equals_WithAccents(string delimited, string regular)
        {
            var regularIdentifier = new SqlIdentifier(regular, false);
            var delimitedIdentifier = new SqlIdentifier(delimited, true);

            Assert.False(regularIdentifier.Equals((object)delimitedIdentifier, CollationInfo.OrdinalIgnoreCase));
            Assert.False(regularIdentifier.Equals((object)delimitedIdentifier.Identifier, CollationInfo.OrdinalIgnoreCase));
            Assert.False(regularIdentifier.Equals(delimitedIdentifier.Identifier, CollationInfo.OrdinalIgnoreCase));
            Assert.False(delimitedIdentifier.Equals((object)regularIdentifier, CollationInfo.OrdinalIgnoreCase));
            Assert.False(delimitedIdentifier.Equals((object)regularIdentifier.Identifier, CollationInfo.OrdinalIgnoreCase));
            Assert.False(delimitedIdentifier.Equals(regularIdentifier.Identifier, CollationInfo.OrdinalIgnoreCase));
        }

        [Theory]
        [MemberData(nameof(Latin1_CS_AS_Identifiers))]
        public void Ordinal_CaseSensitive_Equals(string delimited, string regular)
        {
            var regularIdentifier = new SqlIdentifier(regular, false);
            var delimitedIdentifier = new SqlIdentifier(delimited, true);

            Assert.True(regularIdentifier.Equals((object)delimitedIdentifier, CollationInfo.Ordinal));
            Assert.True(regularIdentifier.Equals((object)delimitedIdentifier.Identifier, CollationInfo.Ordinal));
            Assert.True(regularIdentifier.Equals(delimitedIdentifier.Identifier, CollationInfo.Ordinal));
            Assert.True(delimitedIdentifier.Equals((object)regularIdentifier, CollationInfo.Ordinal));
            Assert.True(delimitedIdentifier.Equals((object)regularIdentifier.Identifier, CollationInfo.Ordinal));
            Assert.True(delimitedIdentifier.Equals(regularIdentifier.Identifier, CollationInfo.Ordinal));
        }

        [Theory]
        [MemberData(nameof(Latin1_CI_AS_Identifiers))]
        [MemberData(nameof(Latin1_CI_AI_Identifiers))]
        public void Ordinal_CaseSensitive_Equals_WithAccents_AndCaseInsensitive(string delimited, string regular)
        {
            var regularIdentifier = new SqlIdentifier(regular, false);
            var delimitedIdentifier = new SqlIdentifier(delimited, true);

            Assert.False(regularIdentifier.Equals((object)delimitedIdentifier, CollationInfo.Ordinal));
            Assert.False(regularIdentifier.Equals((object)delimitedIdentifier.Identifier, CollationInfo.Ordinal));
            Assert.False(regularIdentifier.Equals(delimitedIdentifier.Identifier, CollationInfo.Ordinal));
            Assert.False(delimitedIdentifier.Equals((object)regularIdentifier, CollationInfo.Ordinal));
            Assert.False(delimitedIdentifier.Equals((object)regularIdentifier.Identifier, CollationInfo.Ordinal));
            Assert.False(delimitedIdentifier.Equals(regularIdentifier.Identifier, CollationInfo.Ordinal));
        }

        [Theory]
        [MemberData(nameof(Latin1_CS_AS_Identifiers))]
        [MemberData(nameof(Latin1_CI_AS_Identifiers))]
        public void AnotherTypes_Equals(string delimited, string regular)
        {
            var regularIdentifier = new SqlIdentifier(regular, false);
            var delimitedIdentifier = new SqlIdentifier(delimited, true);

            Assert.False(regularIdentifier.Equals(new object()));
            Assert.False(regularIdentifier.Equals(0));
            Assert.False(delimitedIdentifier.Equals(new object()));
            Assert.False(delimitedIdentifier.Equals(0));
        }

        [Theory]
        [MemberData(nameof(Latin1_CI_AS_Identifiers))]
        [MemberData(nameof(Latin1_CS_AS_Identifiers))]
        [MemberData(nameof(Latin1_CI_AI_Identifiers))]
        public void NullCollation_Equals(string delimited, string regular)
        {
            var regularIdentifier = new SqlIdentifier(regular, false);
            var delimitedIdentifier = new SqlIdentifier(delimited, true);

            Assert.True(regularIdentifier.Equals((object)regularIdentifier, null));
            Assert.Throws<ArgumentNullException>(() => regularIdentifier.Equals((object)delimitedIdentifier, null));
            Assert.Throws<ArgumentNullException>(() => regularIdentifier.Equals((object)delimitedIdentifier.Identifier, null));
            Assert.True(delimitedIdentifier.Equals((object)delimitedIdentifier, null));
            Assert.Throws<ArgumentNullException>(() => delimitedIdentifier.Equals((object)regularIdentifier, null));
            Assert.Throws<ArgumentNullException>(() => delimitedIdentifier.Equals((object)regularIdentifier.Identifier, null));
        }

        [Theory]
        [MemberData(nameof(Latin1_CS_AS_Identifiers))]
        [MemberData(nameof(Latin1_CI_AS_Identifiers))]
        public void Equals_Operator(string delimited, string regular)
        {
            var regularIdentifier = new SqlIdentifier(regular, false);
            var delimitedIdentifier = new SqlIdentifier(delimited, true);

            Assert.True(regularIdentifier == delimitedIdentifier);
            Assert.True(delimitedIdentifier == regularIdentifier);
        }

        [Theory]
        [MemberData(nameof(Latin1_CS_AS_Identifiers))]
        [MemberData(nameof(Latin1_CI_AS_Identifiers))]
        public void Equals_Operator_WithNullOperand(string delimited, string regular)
        {
            var regularIdentifier = new SqlIdentifier(regular, false);
            var delimitedIdentifier = new SqlIdentifier(delimited, true);

            Assert.False(regularIdentifier == null);
            Assert.False(null == regularIdentifier);
            Assert.False(delimitedIdentifier == null);
            Assert.False(null == delimitedIdentifier);
            Assert.True((SqlIdentifier)null == (SqlIdentifier)null);
        }

        [Theory]
        [MemberData(nameof(Latin1_CS_AS_Identifiers))]
        [MemberData(nameof(Latin1_CI_AS_Identifiers))]
        public void NotEquals_Operator(string delimited, string regular)
        {
            var regularIdentifier = new SqlIdentifier(regular, false);
            var delimitedIdentifier = new SqlIdentifier(delimited, true);

            Assert.False(regularIdentifier != delimitedIdentifier);
            Assert.False(delimitedIdentifier != regularIdentifier);
        }

        [Theory]
        [MemberData(nameof(Latin1_CS_AS_Identifiers))]
        [MemberData(nameof(Latin1_CI_AS_Identifiers))]
        public void NotEquals_Operator_WithNullOperand(string delimited, string regular)
        {
            var regularIdentifier = new SqlIdentifier(regular, false);
            var delimitedIdentifier = new SqlIdentifier(delimited, true);

            Assert.True(regularIdentifier != null);
            Assert.True(null != regularIdentifier);
            Assert.True(delimitedIdentifier != null);
            Assert.True(null != delimitedIdentifier);
            Assert.False((SqlIdentifier)null != (SqlIdentifier)null);
        }

        [Fact]
        public void Default_Hashing()
        {
            var items = HashCodeData(x => x.GetHashCode());

            var differentSamples = 0;
            var equalsSamples = 0;

            foreach (var left in items)
            foreach (var right in items)
            {
                if (left.Identifier.Equals(right.Identifier))
                    Assert.Equal(left.HashCode, right.HashCode);
                else if (left.HashCode == right.HashCode)
                    equalsSamples++;
                else
                    differentSamples++;
            }

            Assert.InRange(equalsSamples / (double)(equalsSamples + differentSamples), 0, 0.1);
        }

        [Fact]
        public void Ordinal_CaseInsensitive_Hashing()
        {
            var collation = CollationInfo.OrdinalIgnoreCase;
            var items = HashCodeData(x => x.GetHashCode(collation));

            var differentSamples = 0;
            var equalsSamples = 0;

            foreach (var left in items)
            foreach (var right in items)
            {
                if (left.Identifier.Equals(right.Identifier, collation))
                    Assert.Equal(left.HashCode, right.HashCode);
                else if (left.HashCode == right.HashCode)
                    equalsSamples++;
                else
                    differentSamples++;
            }

            Assert.InRange(equalsSamples / (double)(equalsSamples + differentSamples), 0, 0.1);
        }

        [Fact]
        public void Ordinal_CaseSensitive_Hashing()
        {
            var collation = CollationInfo.Ordinal;
            var items = HashCodeData(x => x.GetHashCode(collation));

            var differentSamples = 0;
            var equalsSamples = 0;

            foreach (var left in items)
            foreach (var right in items)
            {
                if (left.Identifier.Equals(right.Identifier, collation))
                    Assert.Equal(left.HashCode, right.HashCode);
                else if (left.HashCode == right.HashCode)
                    equalsSamples++;
                else
                    differentSamples++;
            }

            Assert.InRange(equalsSamples / (double)(equalsSamples + differentSamples), 0, 0.1);
        }

        [Theory]
        [MemberData(nameof(Latin1_CS_AS_Identifiers))]
        [MemberData(nameof(Latin1_CI_AS_Identifiers))]
        [MemberData(nameof(Latin1_CI_AI_Identifiers))]
        public void NullCollation_Hashing(string delimited, string regular)
        {
            var regularIdentifier = new SqlIdentifier(regular, false);
            var delimitedIdentifier = new SqlIdentifier(delimited, true);

            Assert.Throws<ArgumentNullException>(() => regularIdentifier.GetHashCode(null));
            Assert.Throws<ArgumentNullException>(() => delimitedIdentifier.GetHashCode(null));
        }

        [Theory]
        [MemberData(nameof(Latin1_CS_AS_Identifiers))]
        public void Stringify(string delimited, string regular)
        {
            var regularIdentifier = new SqlIdentifier(regular, false);
            var delimitedIdentifier = new SqlIdentifier(delimited, true);

            Assert.Equal(delimited, regularIdentifier.ToString());
            Assert.Equal(delimited, delimitedIdentifier.ToString());
        }
    }
}
