// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using TSqlCoverage.XEvents.Predicates;
using Xunit;

namespace TSqlCoverage.Core.UnitTests.XEvents.Predicates
{
    public class XEventNotPredicateTests
    {
        [Fact]
        public void Create_NotPredicate_With_Null()
        {
            Assert.Throws<ArgumentNullException>(() => XEventNotPredicate.Create(null));
        }

        [Fact]
        public void Stringfy_NotPredicate()
        {
            var expression = new XEventFakePredicate();
            var predicate = XEventNotPredicate.Create(expression);

            Assert.Equal("NOT (FAKE)", predicate.ToString());
        }

        [Fact]
        public void Stringfy_Double_NotPredicate()
        {
            var expression = new XEventFakePredicate();
            var predicate1 = XEventNotPredicate.Create(expression);
            var predicate2 = XEventNotPredicate.Create(predicate1);

            Assert.Equal("NOT (NOT (FAKE))", predicate2.ToString());
        }

        [Fact]
        public void Stringfy_Triple_NotPredicate()
        {
            var expression = new XEventFakePredicate();
            var predicate1 = XEventNotPredicate.Create(expression);
            var predicate2 = XEventNotPredicate.Create(predicate1);
            var predicate3 = XEventNotPredicate.Create(predicate2);

            Assert.Equal("NOT (NOT (NOT (FAKE)))", predicate3.ToString());
        }

        [Fact]
        public void Optimize_NotPredicate()
        {
            var expression = new XEventFakePredicate();
            var predicate = XEventNotPredicate.Create(expression);

            var optimizedPredicate = predicate.Optimize();

            Assert.Same(predicate, optimizedPredicate);
        }

        [Fact]
        public void Optimize_Double_NotPredicate()
        {
            var expression = new XEventFakePredicate();
            var predicate1 = XEventNotPredicate.Create(expression);
            var predicate2 = XEventNotPredicate.Create(predicate1);

            var optimizedPredicate = predicate2.Optimize();

            Assert.Same(expression, optimizedPredicate);
        }

        [Fact]
        public void Optimize_Triple_NotPredicate()
        {
            var expression = new XEventFakePredicate();
            var predicate1 = XEventNotPredicate.Create(expression);
            var predicate2 = XEventNotPredicate.Create(predicate1);
            var predicate3 = XEventNotPredicate.Create(predicate2);

            var optimizedPredicate = predicate3.Optimize();

            Assert.Same(predicate1, optimizedPredicate);
        }
    }
}
