using Cache.Abstractions;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using Tools;

namespace Cache.Web
{
    public class RedisHealthService : ICacheService, IRedisHealthService
    {
        private const int _cacheVersion = 2;
        private readonly ConnectionMultiplexer _multiplexer;
        private readonly TimeSpan _defaultInvalidation;

        internal RedisHealthService
        (
            ConnectionMultiplexer connectionMultiplexer,
            int keyInvalidationInMinutes
        )
        {
            _multiplexer = connectionMultiplexer;
            _defaultInvalidation = TimeSpan.FromMinutes(keyInvalidationInMinutes);
        }

        public bool IsHealthy() => _multiplexer != null;

        public async Task<T> GetAsync<T>(CacheKey<T> key)
        {
            if (!IsHealthy())
            {
                return default;
            }
            var serializedValue = await GetSerializedValueAsync(key.Key);
            return serializedValue.HasValue ? Serializer.Deserialize<T>(serializedValue) : default;
        }

        public Task SetAsync<T>(CacheKey<T> key, T nonSerializedObject, CacheInvalidation invalidation)
        {
            if (!IsHealthy())
            {
                return Task.CompletedTask;
            }
            var serialized = Serializer.Serialize(nonSerializedObject);
            return SetSerializedValueAsync(key.Key, serialized, invalidation);
        }

        public async Task ExpireAsync<T>(CacheKey<T> key)
        {
            if (!IsHealthy())
            {
                return;
            }

            await _multiplexer.GetDatabase().KeyDeleteAsync(GetCcMasterStringKey(key.Key));
        }

        private Task<RedisValue> GetSerializedValueAsync(string key)
        {
            return _multiplexer
                .GetDatabase()
                .StringGetAsync(GetCcMasterStringKey(key));
        }
        private Task SetSerializedValueAsync(string key, string value, CacheInvalidation invalidation)
        {
            return invalidation switch
            {
                NeverCacheInvalidation _ => _multiplexer.GetDatabase()
                    .StringSetAsync(GetCcMasterStringKey(key), value),
                DefaultCacheInvalidation _ => _multiplexer.GetDatabase()
                    .StringSetAsync(GetCcMasterStringKey(key), value, _defaultInvalidation),
                DurationCacheInvalidation duration => _multiplexer.GetDatabase()
                    .StringSetAsync(GetCcMasterStringKey(key), value, duration.Duration),
                _ => throw new ApplicationException("unhandled type of cache invalidation")
            };
        }

        private string GetCcMasterStringKey(string key) => $"CC.Master:v{_cacheVersion}:{key}";
    }
}
