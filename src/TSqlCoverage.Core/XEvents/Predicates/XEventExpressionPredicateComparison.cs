// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System.IO;
using TSqlCoverage.XEvents.Internal;

namespace TSqlCoverage.XEvents.Predicates
{
    internal class XEventExpressionPredicateComparison : XEventExpressionPredicateOperator
    {
        private readonly string comparator;

        public XEventExpressionPredicateComparison(string comparator)
        {
            this.comparator = comparator;
        }

        public override void WriteTo(TextWriter writer, XEventField field, XEventValue value)
        {
            field.WriteTo(writer);
            writer.Write(comparator);
            value.WriteTo(writer);
        }

        public override string ToString()
            => comparator;
    }
}
