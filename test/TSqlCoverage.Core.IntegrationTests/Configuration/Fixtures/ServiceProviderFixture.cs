// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace TSqlCoverage.Core.IntegrationTests.Configuration.Fixtures
{
    public class ServiceProviderFixture : IDisposable
    {
        private readonly static ServiceContext GlobalContext = new ServiceContext();
        private readonly IServiceScope scope;
        private bool disposed;

        public ServiceProviderFixture()
        {
            scope = GlobalContext.CreateScope();
        }

        public IServiceProvider ServiceProvider
            => scope.ServiceProvider;

        public void Dispose()
        {
            scope.Dispose();
        }
    }
}
