// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.IO;

namespace TSqlCoverage.XEvents
{
    public partial class XEventField
        : IXEventPredicateFragment
        , IEquatable<XEventField>
    {
        private readonly string identifier;

        internal XEventField(string identifier)
        {
            this.identifier = identifier;
        }

        public void WriteTo(TextWriter writer)
            => writer.Write(identifier);

        public override bool Equals(object obj)
        {
            if (obj is XEventField other)
                return Equals(other);

            return false;
        }

        public bool Equals(XEventField other)
        {
            if (object.ReferenceEquals(this, other))
                return true;

            if (other is null)
                return false;

            return Equals(other.identifier);
        }

        public override int GetHashCode()
            => identifier.GetHashCode();

        public static bool operator ==(XEventField left, XEventField right)
            => object.Equals(left, right);

        public static bool operator !=(XEventField left, XEventField right)
            => !(left == right);
    }

    public class XEventField<TValue> : XEventField
    { 
        internal XEventField(string identifier)
            : base(identifier)
        { }
    }
}
