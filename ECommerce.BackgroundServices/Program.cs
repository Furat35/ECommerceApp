using ECommerce.BackgroundServices.Services.Authentication;
using ECommerce.BackgroundServices.Services.Orders;
using ECommerce.Mail.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<OrderConfirmedService>();
        services.AddHostedService<UserRegisterService>();
        services.AddMailServices();
    })
    .Build();

host.Run();
