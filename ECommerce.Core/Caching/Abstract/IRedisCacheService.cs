using Microsoft.Extensions.Caching.Distributed;

namespace ECommerce.Core.Caching.Abstract
{
    public interface IRedisCacheService
    {
        string GetCacheAsString(string cacheKey);
        void SetCacheAsString(string cacheKey, string value, DistributedCacheEntryOptions options);
        void RemoveCache(string cacheKey);
        bool CacheExists(string cacheKey);
    }
}
