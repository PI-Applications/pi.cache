using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace PI.Cache
{
    public class CacheClient
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<object>> completionSourceCache
            = new ConcurrentDictionary<string, TaskCompletionSource<object>>();

        private readonly ConcurrentDictionary<string, DateTimeOffset> expireCache = new ConcurrentDictionary<string, DateTimeOffset>();

        public async Task<TType> GetItem<TType>(string key, Func<Task<TType>> valueFactory)
        {
            return await GetItem(key, () =>
            {
                Console.WriteLine("Legacy run");

                return Task.Run(async () =>
                {
                    var value = await valueFactory();
                    return new CacheValue<TType>(value);
                });
            });
        }

        public async Task<TType> GetItem<TType>(string key, Func<Task<CacheValue<TType>>> valueFactory)
        {
            var newSource = new TaskCompletionSource<object>();
            var currentSource = completionSourceCache.GetOrAdd(key, newSource);

            if (currentSource != newSource)
            {
                bool returnCache = true;

                if (expireCache.TryGetValue(key, out DateTimeOffset cacheExpireTime))
                {
                    if (DateTimeOffset.UtcNow > cacheExpireTime)
                    {
                        returnCache = false;
                        Console.WriteLine($"Cache is expired {cacheExpireTime}");
                    }
                }

                if (returnCache)
                {
                    Console.WriteLine("Return from cache");

                    return (TType)await currentSource.Task;
                }
            }

            try
            {
                if (expireCache.TryRemove(key, out DateTimeOffset cacheExpireTime))
                    Console.WriteLine($"Removed expire cache key entry: {cacheExpireTime}");

                Console.WriteLine("Building from cache");

                var result = await valueFactory();
                newSource.SetResult(result.Value);

                if (result.StoreCache && result.Expires.HasValue)
                    expireCache.GetOrAdd(key, result.Expires.Value);

                if (!result.StoreCache)
                    completionSourceCache.TryRemove(key, out newSource);
            }
            catch (Exception e)
            {
                newSource.SetException(e);

                completionSourceCache.TryRemove(key, out newSource);
            }

            return (TType)await newSource.Task;
        }
    }
}
