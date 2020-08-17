// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using TSqlCoverage.XEvents.Internal;

namespace TSqlCoverage.XEvents
{
    partial class XEventField
    {
        /// <summary>
        /// sql_statement_starting - Occurs when a Transact-SQL statement has started.
        /// </summary>
        public static class SqlStatementStarting
        {
            /// <summary>
            /// Indicates whether the starting of the statement triggered a recompile.
            /// </summary>
            public static readonly XEventField<XEventMapValue<StatementStartingState>> State
                = new XEventField<XEventMapValue<StatementStartingState>>("state");

            /// <summary>
            /// The statement line number, in relation to the beginning of the batch.
            /// </summary>
            public static readonly XEventField<XEventInt32Value> LineNumber
                = new XEventField<XEventInt32Value>("line_number");

            /// <summary>
            /// The statement start offset, in relation to the beginning of the batch.
            /// </summary>
            public static readonly XEventField<XEventInt32Value> Offset
                = new XEventField<XEventInt32Value>("offset");

            /// <summary>
            /// The statement end offset, in relation to the beginning of the batch.
            /// The value will be -1 for the last statement.
            /// </summary>
            public static readonly XEventField<XEventInt32Value> OffsetEnd
                = new XEventField<XEventInt32Value>("offset_end");

            /// <summary>
            /// The text of the statement that triggered the event.
            /// </summary>
            public static readonly XEventField<XEventUnicodeStringValue> Statement
                = new XEventField<XEventUnicodeStringValue>("statement");
        }
    }
}
