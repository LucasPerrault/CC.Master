using System.Threading.Tasks;

namespace Cache.Abstractions
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(CacheKey<T> key);
        Task SetAsync<T>(CacheKey<T> key, T value);
        Task ExpireAsync<T>(CacheKey<T> key);
    }
}
