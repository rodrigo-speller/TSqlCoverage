// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System.IO;
using TSqlCoverage.XEvents.Internal;

namespace TSqlCoverage.XEvents.Predicates
{
    internal abstract class XEventExpressionPredicateOperator
    {
        public static readonly XEventExpressionPredicateComparison Equal
            = new XEventExpressionPredicateComparison("=");
        public static readonly XEventExpressionPredicateComparison NotEqual
            = new XEventExpressionPredicateComparison("<>");
        public static readonly XEventExpressionPredicateComparison GreaterThan
            = new XEventExpressionPredicateComparison(">");
        public static readonly XEventExpressionPredicateComparison GreaterThanOrEqual
            = new XEventExpressionPredicateComparison(">=");
        public static readonly XEventExpressionPredicateComparison LessThan
            = new XEventExpressionPredicateComparison("<");
        public static readonly XEventExpressionPredicateComparison LessThanOrEqual
            = new XEventExpressionPredicateComparison("<=");
        
        /// <summary>
        /// LIKE operator between two SQL UNICODE string values.
        /// </summary>
        public static readonly XEventPredicateExpressionOperator<XEventUnicodeStringValue> LikeISqlUnicodeString
            = new XEventExpressionPredicateFunction<XEventUnicodeStringValue>("sqlserver.like_i_sql_unicode_string");
        
        public abstract void WriteTo(TextWriter writer, XEventField field, XEventValue value);
    }

    internal abstract class XEventPredicateExpressionOperator<TValue>
        : XEventExpressionPredicateOperator where TValue : XEventValue
    {
        public sealed override void WriteTo(TextWriter writer, XEventField field, XEventValue value)
            => WriteTo(writer, (XEventField<TValue>)field, (TValue)value);

        public abstract void WriteTo(TextWriter writer, XEventField<TValue> field, TValue value);
    }
}
