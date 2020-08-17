// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

namespace TSqlCoverage.XEvents
{
    /// <summary>
    /// sql_statement_starting.state values.
    /// </summary>
    /// <seealso cref="XEventField.SqlStatementStarting.State" />
    public enum StatementStartingState
    {
        /// <summary>
        /// Normal.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Recompiled.
        /// </summary>
        Recompiled = 1,

        /// <summary>
        /// Execution Plan Flush.
        /// </summary>
        ExecutionPlanFlush = 2
    }
}
