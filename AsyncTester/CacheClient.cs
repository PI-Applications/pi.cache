using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AsyncTester
{
    public class CacheClient<TKey>
    {
        private readonly ConcurrentDictionary<TKey, TaskCompletionSource<object>> completionSourceCache = new ConcurrentDictionary<TKey, TaskCompletionSource<object>>();

        public async Task<TType> GetItem<TType>(TKey key, Func<Task<TType>> valueFactory) where TType : class
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
