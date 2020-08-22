// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.IO;

namespace TSqlCoverage.XEvents.Predicates
{
    internal sealed class XEventAndPredicate : XEventPredicate
    {
        private XEventAndPredicate(XEventPredicate left, XEventPredicate right)
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

            return new XEventAndPredicate(left, right);
        }

        public override XEventPredicate Optimize()
        {
            var left = Left.Optimize();
            var right = Right.Optimize();

            if (left == right)
                return left;

            var result = new XEventAndPredicate(left, right);
            if (result == this)
                return this;

            return result;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            if (obj is XEventAndPredicate other)
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

        public override void WriteTo(TextWriter writer)
        {
            WriteOperand(writer, Left);
            writer.Write(" AND ");
            WriteOperand(writer, Right);
        }

        private static void WriteOperand(TextWriter writer, XEventPredicate operand)
        {
            if (operand is XEventOrPredicate)
            {
                writer.Write('(');
                operand.WriteTo(writer);
                writer.Write(')');
            }
            else
            {
                operand.WriteTo(writer);
            }
        }
    }
}
