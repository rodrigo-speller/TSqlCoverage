// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using TSqlCoverage.XEvents.Predicates;
using Xunit;

namespace TSqlCoverage.Core.UnitTests.XEvents.Predicates
{
    public class XEventAndPredicateTests
    {
        [Fact]
        public void Create_AndPredicate_With_Nulls()
        {
            Assert.Throws<ArgumentNullException>(() => XEventAndPredicate.Create(null, new XEventFakePredicate()));
            Assert.Throws<ArgumentNullException>(() => XEventAndPredicate.Create(new XEventFakePredicate(), null));
        }

        [Fact]
        public void Stringfy_AndPredicate_With_Different_Operands()
        {
            var left = new XEventFakePredicate("LEFT");
            var right = new XEventFakePredicate("RIGHT");

            var predicate = XEventAndPredicate.Create(left, right);

            Assert.Equal("LEFT AND RIGHT", predicate.ToString());
        }

        [Fact]
        public void Stringfy_AndPredicate_With_Same_Operands()
        {
            var operand = new XEventFakePredicate("OPERAND");

            var predicate = XEventAndPredicate.Create(operand, operand);

            Assert.Equal("OPERAND AND OPERAND", predicate.ToString());
        }

        [Fact]
        public void Optimize_AndPredicate_With_Different_Operands()
        {
            var left = new XEventFakePredicate("LEFT");
            var right = new XEventFakePredicate("RIGHT");

            var predicate = XEventAndPredicate.Create(left, right);
            var optimizedPredicate = predicate.Optimize();

            Assert.Same(predicate, optimizedPredicate);
        }

        [Fact]
        public void Optimize_AndPredicate_With_Equals_Operands()
        {
            var left = new XEventFakePredicate("OPERAND");
            var right = new XEventFakePredicate("OPERAND");

            var predicate = XEventAndPredicate.Create(left, right);
            var optimizedPredicate = predicate.Optimize();

            Assert.Same(left, optimizedPredicate);
        }

        [Fact]
        public void Optimize_AndPredicate_With_Same_Operands()
        {
            var operand = new XEventFakePredicate("OPERAND");

            var predicate = XEventAndPredicate.Create(operand, operand);
            var optimizedPredicate = predicate.Optimize();

            Assert.Same(operand, optimizedPredicate);
        }
    }
}
