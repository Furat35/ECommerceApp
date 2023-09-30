using ECommerce.Core.Caching;
using ECommerce.Core.Caching.Abstract;
using ECommerce.Core.Caching.Concrete;
using ECommerce.Core.RabbitMqMessageBroker;
using ECommerce.Core.RabbitMqMessageBroker.Abstract;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ECommerce.Core.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddCoreLayerServices(this IServiceCollection services)
        {
            #region Redis Configuration
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "127.0.0.1:6379";
            });
            #endregion

            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IRabbitMqMessageConsumer, RabbitMqMessageConsumer>();
            services.AddScoped<IRabbitMqMessagePublisher, RabbitMqMessagePublisher>();

            #region FluentValidation
            services.AddFluentValidation(_ => _.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));
            #endregion
        }
    }
}
