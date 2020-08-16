// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.IO;
using TSqlCoverage.XEvents.Internal;

namespace TSqlCoverage.XEvents.Predicates
{
    internal class XEventExpressionPredicateFunction<TValue>
        : XEventPredicateExpressionOperator<TValue> where TValue : XEventValue
    {
        private readonly string identifier;

        public XEventExpressionPredicateFunction(string identifier)
        {
            this.identifier = identifier;
        }

        public override void WriteTo(TextWriter writer, XEventField<TValue> field, TValue value)
        {
            writer.Write(identifier);
            writer.Write('(');
            field.WriteTo(writer);
            writer.Write(", ");
            value.WriteTo(writer);
            writer.Write(')');
        }
    }
}
