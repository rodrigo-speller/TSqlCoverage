// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.IO;

namespace TSqlCoverage.XEvents.Predicates
{
    internal sealed class XEventOrPredicate : XEventPredicate
    {
        private XEventOrPredicate(XEventPredicate left, XEventPredicate right)
        {
            Left = left;
            Right = right;
        }

        public XEventPredicate Left { get; private set; }
        public XEventPredicate Right { get; private set; }

        public static XEventPredicate Create(XEventPredicate left, XEventPredicate right)
        {
            if (left is null)
                throw new ArgumentNullException(nameof(left));

            if (right is null)
                throw new ArgumentNullException(nameof(right));

            if (left == right)
                return left;

            return new XEventOrPredicate(left, right);
        }

        public override void WriteTo(TextWriter writer)
        {
            Left.WriteTo(writer);
            writer.Write(" OR ");
            Right.WriteTo(writer);
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            if (obj is XEventOrPredicate other)
            {
                var result = this.Left == other.Left && this.Right == other.Right
                    || this.Left == other.Right && this.Right == other.Left
                ;

                if (result)
                {
                    // to improve performante in future comparisons
                    other.Left = this.Left;
                    other.Right = this.Right;
                }

                return result;
            }

            return false;
        }

        public override int GetHashCode()
            => (Left, Right).GetHashCode();
    }
}
