// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.IO;

namespace TSqlCoverage.XEvents.Predicates
{
    internal sealed class XEventNotPredicate : XEventPredicate
    {
        private XEventNotPredicate(XEventPredicate predicate)
        {
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        public XEventPredicate Predicate { get; private set; }

        public static XEventPredicate Create(XEventPredicate predicate)
        {
            if (predicate is XEventNotPredicate not)
                return not.Predicate;

            return new XEventNotPredicate(predicate);
        }

        public override void WriteTo(TextWriter writer)
        {
            writer.Write("NOT (");
            Predicate.WriteTo(writer);
            writer.Write(')');
        }
        
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            if (obj is XEventNotPredicate other)
            {
                if (this.Predicate == other.Predicate)
                {
                    // to improve performante in future comparisons
                    other.Predicate = this.Predicate;

                    return true;
                }

                return false;
            }

            return false;
        }

        public override int GetHashCode()
            => ~Predicate.GetHashCode();
    }
}
