using ECommerce.Core.Caching.Abstract;
using Microsoft.Extensions.Caching.Distributed;

namespace ECommerce.Core.Caching.Concrete
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public bool CacheExists(string cacheKey)
            => _cache.GetString(cacheKey) != null ? true : false;

        public string GetCacheAsString(string cacheKey)
            => _cache.GetString(cacheKey);

        public void RemoveCache(string cacheKey)
        {
            if (CacheExists(cacheKey))
                _cache.Remove(cacheKey);
        }

        public void SetCacheAsString(string cacheKey, string value, DistributedCacheEntryOptions options)
        {
            if (options != null)
                _cache.SetString(cacheKey, value, options);
            else
                _cache.SetString(cacheKey, value);

        }
    }
}
