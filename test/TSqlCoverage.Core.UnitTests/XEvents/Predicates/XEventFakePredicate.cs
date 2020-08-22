// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System.IO;
using TSqlCoverage.XEvents;

namespace TSqlCoverage.Core.UnitTests.XEvents.Predicates
{
    internal class XEventFakePredicate : XEventPredicate
    {
        private readonly string expression;
        public XEventFakePredicate()
            : this("FAKE")
        { }

        public XEventFakePredicate(string expression)
        {
            this.expression = expression;
        }

        public override void WriteTo(TextWriter writer)
            => writer.Write(expression);

        public override bool Equals(object obj)
        {
            if (obj is XEventFakePredicate other)
                return other.expression == this.expression;

            return false;
        }

        public override int GetHashCode()
            => expression?.GetHashCode() ?? 0.GetHashCode();
    }
}
