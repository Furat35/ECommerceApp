using ECommerce.Core.Caching.Abstract;
using Microsoft.Extensions.Caching.Distributed;

namespace ECommerce.Core.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IRedisCacheService _cache;

        public CacheService(IRedisCacheService cache)
        {
            _cache = cache;
        }

        public bool CacheExists(string cacheKey)
            => _cache.CacheExists(cacheKey);

        public string GetCacheAsString(string cacheKey)
            => _cache.GetCacheAsString(cacheKey);

        public void RemoveCache(string cacheKey)
        {
            if (_cache.CacheExists(cacheKey))
                _cache.RemoveCache(cacheKey);
        }

        public void SetCacheAsString(string cacheKey, string value, DistributedCacheEntryOptions options)
            => _cache.SetCacheAsString(cacheKey, value, options);
    }
}
