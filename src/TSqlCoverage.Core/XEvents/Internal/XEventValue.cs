// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.IO;

namespace TSqlCoverage.XEvents.Internal
{
    
    public abstract class XEventValue : IXEventPredicateFragment
    {
        public override bool Equals(object obj)
            => base.Equals(obj);

        public override int GetHashCode()
            => base.GetHashCode();

        public abstract void WriteTo(TextWriter writer);

        public static bool operator ==(XEventValue left, XEventValue right)
            => object.Equals(left, right);

        public static bool operator !=(XEventValue left, XEventValue right)
            => !(left == right);
    }

    public abstract class XEventValue<TValue>
        : XEventValue
        , IEquatable<XEventValue<TValue>>
    {
        internal XEventValue(TValue value)
        {
            Value = value;
        }

        protected TValue Value { get; }

        public override bool Equals(object obj)
        {
            if (obj is XEventValue<TValue> other)
                return Equals(other);
    
            return false;
        }

        public bool Equals(XEventValue<TValue> other)
        {
            if (object.ReferenceEquals(this, other))
                return true;

            if (other is null)
                return false;

            return object.Equals(this.Value, other.Value);
        }

        public override int GetHashCode()
            => Value?.GetHashCode() ?? 0.GetHashCode();
    }
}
