// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.IO;

namespace TSqlCoverage.XEvents.Internal
{
    public sealed class XEventUnicodeStringValue : XEventValue<string>
    {
        internal XEventUnicodeStringValue(string value)
            : base(value ?? throw new ArgumentNullException(nameof(value)))
        { }

        public override void WriteTo(TextWriter writer)
        {
            var value = Value;

            writer.Write("N\'");

            var currentIndex = 0;
            var quoteIndex = Value.IndexOf('\'');
            
            while(quoteIndex >= 0)
            {
                var partialValue = value.Substring(currentIndex, quoteIndex - currentIndex);
                writer.Write(partialValue);
                writer.Write("''");

                currentIndex = quoteIndex + 1;

                if (currentIndex == value.Length)
                    break;
                
                quoteIndex = Value.IndexOf('\'', currentIndex);
            }

            if (currentIndex == 0)
            {
                writer.Write(value);
            }
            else if (currentIndex != value.Length)
            {
                var partialValue = value.Substring(currentIndex, value.Length - currentIndex);
                writer.Write(partialValue);
            }

            writer.Write('\'');
        }

        public static implicit operator XEventUnicodeStringValue(string value)
            => new XEventUnicodeStringValue(value);
    }
}
