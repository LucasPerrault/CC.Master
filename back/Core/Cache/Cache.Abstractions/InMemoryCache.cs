using System;
using System.Collections.Concurrent;

namespace Cache.Abstractions
{
    public abstract class InMemoryCache<TKey, TValue> where TValue : class
    {
        private readonly TimeSpan _cacheInvalidation;
        private readonly ConcurrentDictionary<TKey, CachedValue> _cache;

        protected InMemoryCache(TimeSpan cacheInvalidation)
        {
            _cacheInvalidation = cacheInvalidation;
            _cache = new ConcurrentDictionary<TKey, CachedValue>();
        }

        public bool TryGet(TKey key, out TValue value)
        {
            if (!_cache.TryGetValue(key, out CachedValue cachedValue))
            {
                value = null;
                return false;
            }

            value = GetValid(cachedValue);
            return value != null;
        }

        public  void Cache(TKey key, TValue value)
        {
            _cache[key] = new CachedValue(value, _cacheInvalidation);
        }

        private TValue GetValid(CachedValue cachedPrincipal)
        {
            return cachedPrincipal.InvalidationDate < DateTime.Now
                ? null
                : cachedPrincipal.Value;
        }

        private class CachedValue
        {
            public TValue Value { get; }
            public DateTime InvalidationDate { get; }

            public CachedValue(TValue value, TimeSpan validityDuration)
            {
                Value = value;
                InvalidationDate = DateTime.Now.Add(validityDuration);
            }
        }
    }
}
