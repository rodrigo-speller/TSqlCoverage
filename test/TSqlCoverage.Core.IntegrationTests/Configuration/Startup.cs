// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace TSqlCoverage.Core.IntegrationTests.Configuration
{
    internal class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<Startup>();
        }
    }
}
