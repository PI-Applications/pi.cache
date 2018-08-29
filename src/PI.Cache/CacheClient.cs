using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace PI.Cache
{
    public class CacheClient
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<object>> completionSourceCache 
            = new ConcurrentDictionary<string, TaskCompletionSource<object>>();

        public async Task<TType> GetItem<TType>(string key, Func<Task<TType>> valueFactory) where TType : class
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

        public async Task<TType> GetItem<TType>(string key, Func<Task<CacheValue<TType>>> valueFactory)
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
                newSource.SetResult(result.Value);

                if (!result.StoreCache)
                    completionSourceCache.TryRemove(key, out newSource);
            }
            catch (Exception e)
            {
                newSource.SetException(e);
            } 

            return (TType)await newSource.Task;
        }
    }
}
