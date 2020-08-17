// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.Globalization;
using System.IO;

namespace TSqlCoverage.XEvents.Internal
{
    public sealed class XEventMapValue<TValue> : XEventValue<TValue> where TValue : Enum
    {
        internal XEventMapValue(TValue value)
            : base(value)
        { }

        public override void WriteTo(TextWriter writer)
        {
            var key = Convert.ToInt32(this.Value);

            var value = key.ToString(CultureInfo.InvariantCulture);
            writer.Write(value);
        }

        public static implicit operator XEventMapValue<TValue>(TValue value)
            => new XEventMapValue<TValue>(value);
    }
}
