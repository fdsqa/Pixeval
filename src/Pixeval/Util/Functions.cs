﻿using System;
using System.Threading.Tasks;

namespace Pixeval.Util
{
    public static class Functions
    {
        public static void IgnoreException(Action action)
        {
            try
            {
                action();
            }
            catch
            {
                // ignore
            }
        }

        public static async Task IgnoreExceptionAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch
            {
                // ignore
            }
        }

        public static Task<TResult> TryCatchAsync<TResult>(Func<Task<TResult>> function, Func<Exception, Task<TResult>> onException)
        {
            try
            {
                return function();
            }
            catch (Exception e)
            {
                return onException(e);
            }
        }

        public static TResult Block<TResult>(Func<TResult> block)
        {
            return block();
        }

        public static Task<TResult> BlockAsync<TResult>(Func<Task<TResult>> block)
        {
            return block();
        }
    }
}