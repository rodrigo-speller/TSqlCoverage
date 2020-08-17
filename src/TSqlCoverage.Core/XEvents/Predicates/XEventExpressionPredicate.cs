// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.IO;
using TSqlCoverage.XEvents.Internal;

namespace TSqlCoverage.XEvents.Predicates
{
    internal class XEventExpressionPredicate : XEventPredicate
    {
        private XEventExpressionPredicate(
            XEventExpressionPredicateOperator @operator,
            XEventField field,
            XEventValue value)
        {
            Field = field;
            Operator = @operator;
            Value = value;
        }

        public XEventField Field { get; private set; }
        public XEventExpressionPredicateOperator Operator { get; private set; }
        public XEventValue Value { get; private set; }

        private static XEventPredicate Create<TValue, TRawValue>(
            XEventPredicateExpressionOperator<TValue> @operator,
            XEventField<TValue> field,
            TValue value
        ) where TValue : XEventValue<TRawValue>
            => new XEventExpressionPredicate(
                @operator ?? throw new ArgumentNullException(nameof(@operator)),
                field ?? throw new ArgumentNullException(nameof(field)),
                value ?? throw new ArgumentNullException(nameof(value))
            );

        private static XEventPredicate Create<TValue, TRawValue>(
            XEventExpressionPredicateComparison @operator,
            XEventField<TValue> field,
            TValue value
        ) where TValue : XEventValue<TRawValue>
            => new XEventExpressionPredicate(
                @operator ?? throw new ArgumentNullException(nameof(@operator)),
                field ?? throw new ArgumentNullException(nameof(field)),
                value ?? throw new ArgumentNullException(nameof(value))
            );

        public static XEventPredicate Create(
            XEventExpressionPredicateComparison @operator,
            XEventField<XEventUnicodeStringValue> field,
            string value
        )
            => Create<XEventUnicodeStringValue, string>(@operator, field, value);

        public static XEventPredicate Create(
            XEventExpressionPredicateComparison @operator,
            XEventField<XEventInt32Value> field,
            int value
        )
            => Create<XEventInt32Value, int>(@operator, field, value);

        public static XEventPredicate Create(
            XEventExpressionPredicateComparison @operator,
            XEventField<XEventUInt64Value> field,
            ulong value
        )
            => Create<XEventUInt64Value, ulong>(@operator, field, value);

        public static XEventPredicate Create<T>(
            XEventExpressionPredicateComparison @operator,
            XEventField<XEventMapValue<T>> field,
            T value
        ) where T : Enum
            => Create<XEventMapValue<T>, T>(@operator, field, value);

        public static XEventPredicate Create(
            XEventPredicateExpressionOperator<XEventUnicodeStringValue> @operator,
            XEventField<XEventUnicodeStringValue> field,
            string value
        )
            => Create<XEventUnicodeStringValue, string>(@operator, field, value);

        public override void WriteTo(TextWriter writer)
            => Operator.WriteTo(writer, Field, Value);

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            if (obj is XEventExpressionPredicate other)
            {
                var result = this.Operator == other.Operator
                    && this.Field == other.Field
                    && this.Value == other.Value
                ;

                if (result)
                {
                    // to improve performante in future comparisons
                    other.Operator = this.Operator;
                    other.Field = this.Field;
                    other.Value = this.Value;
                }

                return result;
            }

            return false;
        }

        public override int GetHashCode()
            => (Operator, Field, Value).GetHashCode();
    }
}
