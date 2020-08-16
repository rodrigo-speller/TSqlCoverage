// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using TSqlCoverage.XEvents.Internal;
using TSqlCoverage.XEvents.Predicates;

namespace TSqlCoverage.XEvents
{
    public static class XEventPredicateExtensions
    {
        public static XEventPredicate Equal(this XEventField<XEventUInt64Value> field, ulong value)
            => XEventExpressionPredicate.Create(XEventExpressionPredicateComparison.Equal, field, value);
        public static XEventPredicate NotEqual(this XEventField<XEventUInt64Value> field, ulong value)
            => XEventExpressionPredicate.Create(XEventExpressionPredicateComparison.NotEqual, field, value);
        public static XEventPredicate GreaterThan(this XEventField<XEventUInt64Value> field, ulong value)
            => XEventExpressionPredicate.Create(XEventExpressionPredicateComparison.GreaterThan, field, value);
        public static XEventPredicate GreaterThanOrEqual(this XEventField<XEventUInt64Value> field, ulong value)
            => XEventExpressionPredicate.Create(XEventExpressionPredicateComparison.GreaterThanOrEqual, field, value);
        public static XEventPredicate LessThan(this XEventField<XEventUInt64Value> field, ulong value)
            => XEventExpressionPredicate.Create(XEventExpressionPredicateComparison.LessThan, field, value);
        public static XEventPredicate LessThanOrEqual(this XEventField<XEventUInt64Value> field, ulong value)
            => XEventExpressionPredicate.Create(XEventExpressionPredicateComparison.LessThanOrEqual, field, value);

        public static XEventPredicate Equal(this XEventField<XEventUnicodeStringValue> field, string value)
            => XEventExpressionPredicate.Create(XEventExpressionPredicateComparison.Equal, field, value);
        public static XEventPredicate NotEqual(this XEventField<XEventUnicodeStringValue> field, string value)
            => XEventExpressionPredicate.Create(XEventExpressionPredicateComparison.NotEqual, field, value);
        public static XEventPredicate GreaterThan(this XEventField<XEventUnicodeStringValue> field, string value)
            => XEventExpressionPredicate.Create(XEventExpressionPredicateComparison.GreaterThan, field, value);
        public static XEventPredicate GreaterThanOrEqual(this XEventField<XEventUnicodeStringValue> field, string value)
            => XEventExpressionPredicate.Create(XEventExpressionPredicateComparison.GreaterThanOrEqual, field, value);
        public static XEventPredicate LessThan(this XEventField<XEventUnicodeStringValue> field, string value)
            => XEventExpressionPredicate.Create(XEventExpressionPredicateComparison.LessThan, field, value);
        public static XEventPredicate LessThanOrEqual(this XEventField<XEventUnicodeStringValue> field, string value)
            => XEventExpressionPredicate.Create(XEventExpressionPredicateComparison.LessThanOrEqual, field, value);
            
        public static XEventPredicate Like(this XEventField<XEventUnicodeStringValue> field, string value)
            => XEventExpressionPredicate.Create(XEventExpressionPredicateOperator.LikeISqlUnicodeString, field, value);
    }
}
