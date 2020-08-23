// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.SqlServer.Management.SqlParser.Parser;

namespace TSqlCoverage.Metadata
{
    [Serializable]
    public class TSqlParseException : Exception
    {
        private const string DefaultMessage = "An error ocurred while parsing T-SQL code.";

        public TSqlParseException()
            : this(DefaultMessage)
        { }

        public TSqlParseException(string message)
            : base(message ?? DefaultMessage)
        {
            this.Errors = Array.Empty<ErrorInfo>();
        }

        public TSqlParseException(IEnumerable<Error> errors)
            : base("One or more errors occurred while parsing T-SQL code.")
        {
            if (errors is null)
                throw new ArgumentNullException(nameof(errors));

            Errors = errors.Select(e => new ErrorInfo(e)).ToArray();
        }

        public TSqlParseException(Exception inner)
            : base("One exception occurred while parsing T-SQL code.", inner)
        {
            this.Errors = Array.Empty<ErrorInfo>();
        }

        protected TSqlParseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Errors = (ErrorInfo[])info.GetValue("Errors", typeof(ErrorInfo[]))
                ?? Array.Empty<ErrorInfo>();
        }

        public IReadOnlyList<ErrorInfo> Errors { get; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Errors", Errors);
        }
    }
}
