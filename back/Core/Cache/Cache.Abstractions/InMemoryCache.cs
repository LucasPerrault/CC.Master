using System;
using System.Collections.Concurrent;
using Tools;

namespace Cache.Abstractions
{
    public abstract class InMemoryCache<TKey, TValue> where TValue : class
    {
        private readonly TimeSpan _cacheInvalidation;
        private readonly ConcurrentDictionary<TKey, CachedValue<TValue>> _cache;

        protected InMemoryCache(TimeSpan cacheInvalidation)
        {
            _cacheInvalidation = cacheInvalidation;
            _cache = new ConcurrentDictionary<TKey, CachedValue<TValue>>();
        }

        public bool HasKey(TKey key)
        {
            return TryGet(key, out _);
        }

        public bool TryGet(TKey key, out TValue value)
        {
            if (!_cache.TryGetValue(key, out var cachedValue))
            {
                value = null;
                return false;
            }

            value = cachedValue.SafeValue;
            return value != null;
        }

        public  void Cache(TKey key, TValue value)
        {
            _cache[key] = new CachedValue<TValue>(value, _cacheInvalidation);
        }
    }

    public abstract class InMemoryValueObjectCache<T>
        where T : ValueObject
    {
        private readonly InMemoryCache<T, T> _cache;

        protected InMemoryValueObjectCache(TimeSpan cacheInvalidation)
        {
            _cache = new PrivateCache(cacheInvalidation);
        }

        public bool Has(T value)
        {
            return _cache.TryGet(value, out _);
        }

        public  void Cache(T value)
        {
            _cache.Cache(value, value);
        }

        private class PrivateCache : InMemoryCache<T, T>
        {
            public PrivateCache(TimeSpan cacheInvalidation) : base(cacheInvalidation)
            { }
        }
    }

    public abstract class InMemoryCache<T> where T : class
    {
        private readonly TimeSpan _cacheInvalidation;
        private CachedValue<T> _cache;
        private readonly object _lock = new object();

        protected InMemoryCache(TimeSpan cacheInvalidation)
        {
            _cacheInvalidation = cacheInvalidation;
        }

        public bool TryGet(out T value)
        {
            lock (_lock)
            {
                if (_cache == null)
                {
                    value = null;
                    return false;
                }

                value = _cache.SafeValue;
                return value != null;
            }
        }

        public  void Cache(T value)
        {
            lock (_lock)
            {
                _cache = new CachedValue<T>(value, _cacheInvalidation);
            }
        }
    }

    internal class CachedValue<TValue> where TValue : class
    {
        public TValue Value { get; }
        private readonly DateTime _invalidationDate;

        public CachedValue(TValue value, TimeSpan validityDuration)
        {
            Value = value;
            _invalidationDate = DateTime.Now.Add(validityDuration);
        }

        public TValue SafeValue => _invalidationDate < DateTime.Now ? null : Value;
    }
}
