// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

namespace TSqlCoverage
{
    public class XEventStatement
    {
        public int DatabaseId { get; set; }
        public int ObjectId { get; set; }
        public string ObjectType { get; set; }
        public int Offset { get; set; }
        public int OffsetEnd { get; set; }
        public string Statement { get; set; }
    }
}
