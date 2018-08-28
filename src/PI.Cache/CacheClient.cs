using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace PI.Cache
{
    public static class CacheClient
    {
        private static readonly ConcurrentDictionary<string, TaskCompletionSource<object>> completionSourceCache = new ConcurrentDictionary<string, TaskCompletionSource<object>>();

        public static async Task<TType> GetItem<TType>(string key, Func<Task<TType>> valueFactory) where TType : class
        {
            var newSource = new TaskCompletionSource<object>();
            var currentSource = completionSourceCache.GetOrAdd(key, newSource);

            if (currentSource != newSource)
            {
                Console.WriteLine("Return from cache");

                return (TType)await currentSource.Task;
            }

            try
            {
                Console.WriteLine("Building from cache");

                var result = await valueFactory();
                newSource.SetResult(result);
            }
            catch (Exception e)
            {
                newSource.SetException(e);
            }

            return (TType)await newSource.Task;
        }
    }
}
