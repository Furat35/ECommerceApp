using ECommerce.Data.Repositories.EntityFrameworkContext;
using ECommerce.Data.UnitOfWorks;
using ECommerce.Entity.Entities.IdentityFrameworkEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Data.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddDataLayerServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region DbContext Configuration
            services.AddDbContext<EfContext>(_ => _.UseSqlServer(configuration.GetConnectionString("Mssql")));
            #endregion

            #region Identity Framework Configuration
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 2;
            })
                .AddEntityFrameworkStores<EfContext>()
                .AddDefaultTokenProviders();
            #endregion

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
