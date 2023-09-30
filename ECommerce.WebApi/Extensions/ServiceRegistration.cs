using ECommerce.Core.ActionFilters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace ECommerce.WebApi.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddApiLayerServices(this IServiceCollection services, WebApplicationBuilder builder, IConfiguration configuration)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ValidateModelStateAttribute));
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddEndpointsApiExplorer();

            #region Swagger Configuration
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "ECommerce",
                    Version = "v1",
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
            #endregion

            services.AddHttpContextAccessor();

            #region jwt Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer("Admin", options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateAudience = true,//olusturulacak token degerini kimlerin/hangi originlerin erisebilecegi
                        ValidateIssuer = true,  //tokeni olusturan
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidAudience = builder.Configuration["JWTAuth:ValidAudienceURL"],
                        ValidIssuer = builder.Configuration["JWTAuth:ValidIssuerURL"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTAuth:SecretKey"])),
                        LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

                        NameClaimType = ClaimTypes.Name
                    };
                });
            #endregion

            #region Cache Configurations
            //services.AddMemoryCache();
            #endregion

            #region Serilog Configuration

            //ColumnOptions columnOpt = new ColumnOptions();
            //columnOpt.Store.Remove(StandardColumn.Properties);
            //columnOpt.Store.Add(StandardColumn.LogEvent);

            using var log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/e-commerce.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.MSSqlServer(
                    builder.Configuration.GetConnectionString("Mssql"),
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        AutoCreateSqlTable = true,
                        TableName = "e-commerce-logs",
                    })
                .Enrich.FromLogContext()
                .MinimumLevel.Information()
                .CreateLogger();

            builder.Host.UseSerilog(log);
            #endregion
        }
    }
}
