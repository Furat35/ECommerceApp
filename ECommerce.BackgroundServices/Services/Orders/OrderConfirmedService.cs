using ECommerce.Core.BackgroundServices.DTOs;
using ECommerce.Core.RabbitMqMessageBroker;
using ECommerce.Mail.Abstract;
using ECommerce.Service.Abstract;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ECommerce.BackgroundServices.Services.Orders
{
    public class OrderConfirmedService : BackgroundService
    {
        private readonly IMailService _mailService;
        private readonly IAppUserService _userService;
        EventingBasicConsumer _consume;
        IModel _channel;
        public OrderConfirmedService(IConfiguration configuration, IMailService mailService)
        {
            RabbitMqMessageConsumer consumer = new RabbitMqMessageConsumer(configuration);
            _channel = consumer.Connect();
            _consume = consumer.ConsumerMessageConfigurations("E.Commerce.Orders.Completed", "E_Commerce_Orders", ExchangeType.Direct, "E.Commerce.Orders.Completed.Key");
            _mailService = mailService;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consume.Received += (sender, e) =>
            {
                var orderDetail = JsonSerializer.Deserialize<OrderCompleted>(Encoding.UTF8.GetString(e.Body.Span));
                Console.WriteLine(orderDetail.OrderId);
                _channel.BasicAck(e.DeliveryTag, multiple: false);
                //_mailService.SendMail($"{orderDetail.Email}", "Sipariþiniz Tamamlanmýþtýr", $"{orderDetail.OrderId} nolu sipariþiniz baþarýyla verilmiþtir.");
            };

            return Task.CompletedTask;
        }
    }
}