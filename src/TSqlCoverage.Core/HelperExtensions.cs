// Copyright (c) Rodrigo Speller. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace TSqlCoverage
{
    internal static class HelperExtensions
    {
        public async static Task<TResult> RequestAsync<TResult>(this SemaphoreSlim semaphore, Func<TResult> action)
        {
            await semaphore.WaitAsync();
            try
            {
                return action();
            }
            finally
            {
                semaphore.Release();
            }
        }

        public static void Execute(this SemaphoreSlim semaphore, Action action)
        {
            semaphore.Wait();
            try
            {
                action();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
