// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System.Runtime.CompilerServices;

#if DEBUG
[assembly: InternalsVisibleTo("TSqlCoverage.Core.IntegrationTests")]
[assembly: InternalsVisibleTo("TSqlCoverage.Core.UnitTests")]
#endif
