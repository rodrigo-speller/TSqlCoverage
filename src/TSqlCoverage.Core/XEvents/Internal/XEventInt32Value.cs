// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System.Globalization;
using System.IO;

namespace TSqlCoverage.XEvents.Internal
{
    public sealed class XEventInt32Value : XEventValue<int>
    {
        internal XEventInt32Value(int value)
            : base(value)
        { }

        public override void WriteTo(TextWriter writer)
        {
            var value = this.Value.ToString(CultureInfo.InvariantCulture);
            writer.Write(value);
        }

        public static implicit operator XEventInt32Value(int value)
            => new XEventInt32Value(value);
    }
}
