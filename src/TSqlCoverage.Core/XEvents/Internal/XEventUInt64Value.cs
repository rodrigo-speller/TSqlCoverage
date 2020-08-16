// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System.Globalization;
using System.IO;

namespace TSqlCoverage.XEvents.Internal
{
    public sealed class XEventUInt64Value : XEventValue<ulong>
    {
        internal XEventUInt64Value(ulong value)
            : base(value)
        { }

        public override void WriteTo(TextWriter writer)
        {
            var value = this.Value.ToString(CultureInfo.InvariantCulture);
            writer.Write(value);
        }

        public static implicit operator XEventUInt64Value(ulong value)
            => new XEventUInt64Value(value);
    }
}
