namespace PI.Cache
{
    public class CacheValue<TValue>
    {
        public CacheValue(TValue value, bool storeCache = true)
        {
            Value = value;
            StoreCache = storeCache;
        }

        public TValue Value { get; set; }
        public bool StoreCache { get; set; }
    }
}
