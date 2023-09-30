﻿using Microsoft.Extensions.Caching.Distributed;

namespace ECommerce.Core.Caching
{
    public interface ICacheService
    {
        string GetCacheAsString(string cacheKey);
        void SetCacheAsString(string cacheKey, string value, DistributedCacheEntryOptions options);
        void RemoveCache(string cacheKey);
        bool CacheExists(string cacheKey);
    }
}
