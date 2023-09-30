using ECommerce.Service.Abstract;
using ECommerce.Service.Concrete;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ECommerce.Service.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddServiceLayerServices(this IServiceCollection services)
        {
            #region AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            #endregion

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IAppRoleService, AppRoleService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAppUserService, AppUserService>();
            services.AddScoped<ILoggerService, LoggerService>(); //todo can't log to database
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IOrderService, OrderService>();
            //services.AddScoped<IRabbitMqMessageService, RabbitMqMessageService>();

            //services.AddSingleton<IHostedService, FireOrderConfirmedService>();
            //services.AddHostedService<FireOrderConfirmedService>();
        }
    }
}
