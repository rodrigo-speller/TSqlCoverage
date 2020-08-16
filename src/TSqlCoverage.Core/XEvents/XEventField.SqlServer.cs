// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using TSqlCoverage.XEvents.Internal;

namespace TSqlCoverage.XEvents
{
    partial class XEventField
    {
        public static class SqlServer
        {
            public static readonly XEventField<XEventUInt64Value> SessionId
                = new XEventField<XEventUInt64Value>("sqlserver.session_id");
            public static readonly XEventField<XEventUInt64Value> DatabaseId
                = new XEventField<XEventUInt64Value>("sqlserver.database_id");
            public static readonly XEventField<XEventUnicodeStringValue> DatabaseName
                = new XEventField<XEventUnicodeStringValue>("sqlserver.database_name");
        }
    }
}
