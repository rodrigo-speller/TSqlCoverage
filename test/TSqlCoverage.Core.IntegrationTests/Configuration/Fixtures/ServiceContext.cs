// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace TSqlCoverage.Core.IntegrationTests.Configuration.Fixtures
{
    public class ServiceContext
    {
        private readonly IServiceProvider rootServices;

        public ServiceContext()
        {
            var collection = new ServiceCollection();

            new Startup().ConfigureServices(collection);

            rootServices = collection.BuildServiceProvider(true);
        }

        public IServiceScope CreateScope()
            => rootServices.CreateScope();
    }
}
