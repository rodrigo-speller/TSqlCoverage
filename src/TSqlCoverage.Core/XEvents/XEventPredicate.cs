// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System.IO;
using TSqlCoverage.XEvents.Predicates;

namespace TSqlCoverage.XEvents
{
    public abstract class XEventPredicate : IXEventPredicateFragment
    {
        internal XEventPredicate()
        { }

        public abstract void WriteTo(TextWriter writer);

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }

        public override bool Equals(object obj)
            => base.Equals(obj);

        public override int GetHashCode()
            => base.GetHashCode();

        public static XEventPredicate operator |(XEventPredicate left, XEventPredicate right)
            => XEventOrPredicate.Create(left, right);

        public static XEventPredicate operator &(XEventPredicate left, XEventPredicate right)
            => XEventAndPredicate.Create(left, right);

        public static XEventPredicate operator !(XEventPredicate predicate)
            => XEventNotPredicate.Create(predicate);

        public static bool operator ==(XEventPredicate left, XEventPredicate right)
            => object.Equals(left, right);

        public static bool operator !=(XEventPredicate left, XEventPredicate right)
            => !(left == right);
    }
}
