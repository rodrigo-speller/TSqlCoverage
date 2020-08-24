// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TSqlCoverage.Core.IntegrationTests.Configuration.Fixtures;
using Xunit;

namespace TSqlCoverage.Core.IntegrationTests
{
    public class TSqlCoverageTracerTests
        : IClassFixture<ServiceProviderFixture>
    {
        public TSqlCoverageTracerTests(ServiceProviderFixture serviceProviderFixture)
        {
            Services = serviceProviderFixture.ServiceProvider;
        }

        public IServiceProvider Services { get; }

        [Fact]
        public async Task Connectivity()
        {
            var configuration = Services.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            using (var tracer = await TSqlCoverageTracer.Start(connectionString, (e) => { /* noop */ }, null))
            using (var command = tracer.CreateCommand())
            {
                command.CommandText = "SELECT 19870412";

                var result = (int)command.ExecuteScalar();

                Assert.Equal(19870412, result);
            }
        }
    }
}
