using System;

namespace PI.Cache
{
    public class CacheValue<TValue>
    {
        public CacheValue(TValue value) : this(value, true, null)
        {
            
        }

        public CacheValue(TValue value, bool storeCache = true, TimeSpan? expires = null)
        {
            Value = value;
            StoreCache = storeCache;

            if (expires != null)
                Expires = DateTimeOffset.UtcNow.Add(expires.Value);
        }

        public TValue Value { get; set; }
        public bool StoreCache { get; set; }
        public DateTimeOffset? Expires { get; set; }
    }
}
