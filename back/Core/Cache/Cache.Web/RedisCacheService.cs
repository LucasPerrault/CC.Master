using Cache.Abstractions;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Cache.Web
{
    public class RedisHealthService : ICacheService, IRedisHealthService
    {
        private const int _cacheVersion = 1;
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
            return serializedValue.HasValue ? JsonConvert.DeserializeObject<T>(serializedValue) : default;
        }

        public Task SetAsync<T>(CacheKey<T> key, T nonSerializedObject)
        {
            if (!IsHealthy())
            {
                return Task.CompletedTask;
            }
            var serialized = JsonConvert.SerializeObject(nonSerializedObject);
            return SetSerializedValueAsync(key.Key, serialized, key.Invalidation);
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
        private Task SetSerializedValueAsync(string key, string value, TimeSpan? invalidation)
        {
            return _multiplexer.GetDatabase().StringSetAsync(GetCcMasterStringKey(key), value, invalidation ?? _defaultInvalidation);
        }

        private string GetCcMasterStringKey(string key) => $"CC.Master:v{_cacheVersion}:{key}";
    }
}