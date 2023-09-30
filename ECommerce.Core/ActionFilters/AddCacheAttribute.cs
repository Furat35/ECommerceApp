using ECommerce.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace ECommerce.Core.ActionFilters
{
    public class AddCacheAttribute : ActionFilterAttribute
    {
        string _cacheName;
        string extension = "GetFilteredResult";
        ICacheService _cacheService;
        public AddCacheAttribute()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _cacheService = (ICacheService)context.HttpContext.RequestServices.GetRequiredService(typeof(ICacheService));
            _cacheName = GetCacheName(context.Controller.ToString());
            object _cacheData = _cacheService.GetCacheAsString(_cacheName);
            if (_cacheData != null && _cacheService
                .GetCacheAsString($"QueryParams-{ActionName(context.HttpContext)}")?.ToString() ==
                context.HttpContext.Request.QueryString.Value)
            {
                context.HttpContext.Response.Headers.Add($"X-Pagination", _cacheService
                    .GetCacheAsString($"X-Pagination-{ActionName(context.HttpContext)}").ToString());
                context.Result = new OkObjectResult(JsonSerializer.Deserialize<object>(_cacheData.ToString()));
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is null)
            {
                base.OnActionExecuted(context);
                _cacheService = (ICacheService)context.HttpContext.RequestServices.GetService(typeof(ICacheService));
                _cacheName = GetCacheName(context.Controller.ToString());
                var resultValue = context.Result.GetType().GetProperty("Value").GetValue(context.Result);
                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(20),
                    SlidingExpiration = TimeSpan.FromMinutes(4),
                };
                _cacheService.SetCacheAsString(_cacheName, JsonSerializer.Serialize(resultValue), options);
                _cacheService.SetCacheAsString($"X-Pagination-{ActionName(context.HttpContext)}", context.HttpContext.Response.Headers["X-Pagination"], options);
                _cacheService.SetCacheAsString($"QueryParams-{ActionName(context.HttpContext)}", context.HttpContext.Request.QueryString.Value, options);
            }
        }

        private string GetCacheName(string controllerName)
            => $"{controllerName}.{extension}";

        private string ActionName(HttpContext httpContext)
            => httpContext.Request.RouteValues.Values.First().ToString();

    }
}
