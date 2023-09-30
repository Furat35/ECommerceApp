using ECommerce.Mail.Abstract;
using ECommerce.Mail.Concrete;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Mail.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddMailServices(this IServiceCollection services)
        {
            services.AddSingleton<IMailService, MailService>();
        }
    }
}
