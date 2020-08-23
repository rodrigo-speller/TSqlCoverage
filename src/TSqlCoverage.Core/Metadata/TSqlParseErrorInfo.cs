// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.Runtime.Serialization;
using Microsoft.SqlServer.Management.SqlParser.Parser;

namespace TSqlCoverage.Metadata
{
    [Serializable]
    public class ErrorInfo : ISerializable
    {
        internal ErrorInfo(Error error)
        {
            Message = error.Message;
            IsWarning = error.IsWarning;
            Type = error.Type;
            Start = error.Start;
            End = error.End;
        }

        internal ErrorInfo(SerializationInfo info, StreamingContext context)
        {
            Message = info.GetString("Message");
            IsWarning = info.GetBoolean("IsWarning");
            Type = (ErrorType)info.GetValue("Type", typeof(ErrorType));
            Start = new Location(
                info.GetInt32("Start.LineNumber"),
                info.GetInt32("Start.ColumnNumber"),
                info.GetInt32("Start.Offset")
            );
            End = new Location(
                info.GetInt32("End.LineNumber"),
                info.GetInt32("End.ColumnNumber"),
                info.GetInt32("End.Offset")
            );
        }

        public string Message { get; }
        public bool IsWarning { get; }
        public ErrorType Type { get; }
        public Location Start { get; }
        public Location End { get; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Message", Message);
            info.AddValue("IsWarning", IsWarning);
            info.AddValue("Type", Type);
            info.AddValue("Start.LineNumber", Start.LineNumber);
            info.AddValue("Start.ColumnNumber", Start.ColumnNumber);
            info.AddValue("Start.Offset", Start.Offset);
            info.AddValue("End.LineNumber", End.LineNumber);
            info.AddValue("End.ColumnNumber", End.ColumnNumber);
            info.AddValue("End.Offset", End.Offset);
        }
    }
}
