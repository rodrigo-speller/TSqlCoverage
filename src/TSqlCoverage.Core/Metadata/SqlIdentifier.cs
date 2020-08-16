// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.SqlParser.Metadata;

namespace TSqlCoverage.Metadata
{
    public sealed class SqlIdentifier
        : IEquatable<SqlIdentifier>
        , IEquatable<string>
    {
        private string unquotedIdentifier;
        private string quotedIdentifier;

        public SqlIdentifier(string identifier, bool isQuoted)
        {
            if (identifier is null)
                throw new ArgumentNullException(nameof(identifier));

            if (isQuoted)
            {
                this.unquotedIdentifier = new SqlCommandBuilder().UnquoteIdentifier(identifier);
                this.quotedIdentifier = identifier;
            }
            else
                this.unquotedIdentifier = identifier;
        }

        public string Identifier => Identifier;

        public string QuotedIdentifier
        {
            get
            {
                var value = this.quotedIdentifier;

                if (value is null)
                    value = this.quotedIdentifier = new SqlCommandBuilder().QuoteIdentifier(this.unquotedIdentifier);

                return value;
            }
        }

        public override int GetHashCode()
            => GetHashCode(CollationInfo.Default);

        public int GetHashCode(CollationInfo collation)
            => (collation ?? CollationInfo.Default).EqualityComparer.GetHashCode(this.unquotedIdentifier);

        public override bool Equals(object obj)
            => Equals(obj, CollationInfo.Default);

        public bool Equals(object obj, CollationInfo collation)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            switch (obj)
            {
                case SqlIdentifier other:
                    return this.Equals(other, collation);
                    
                case string other:
                    return this.Equals(other, collation);
            }

            return false;
        }

        public bool Equals(SqlIdentifier other)
            => Equals(other, CollationInfo.Default);

        public bool Equals(SqlIdentifier other, CollationInfo collation)
            => Equals(other?.unquotedIdentifier, collation);

        public bool Equals(string other)
            => Equals(other, CollationInfo.Default);

        public bool Equals(string other, CollationInfo collation)
        {
            if (collation is null)
                throw new ArgumentNullException(nameof(collation));

            if (other is null)
                return false;

            return collation.EqualityComparer.Equals(this.unquotedIdentifier, other);
        }

        public override string ToString()
            => QuotedIdentifier;

        public static bool operator ==(SqlIdentifier left, SqlIdentifier right)
        {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        public static bool operator !=(SqlIdentifier left, SqlIdentifier right)
            => !(left == right);
    }
}
