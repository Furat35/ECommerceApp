using ECommerce.Core.Caching;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Core.ActionFilters
{
    public class RemoveCacheAttribute : ActionFilterAttribute
    {
        string _cacheName;
        string extension = "GetFilteredResult";
        ICacheService _cacheService;
        public string CacheActionName { get; set; }
        public RemoveCacheAttribute()
        {

        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            _cacheService = (ICacheService)context.HttpContext.RequestServices.GetRequiredService(typeof(ICacheService));
            _cacheName = $"{context.Controller}.{extension}";
            _cacheService.RemoveCache(_cacheName);
            _cacheService.RemoveCache($"QueryParams-{CacheActionName}");
            _cacheService.RemoveCache($"X-Pagination-{CacheActionName}");
        }
    }
}
