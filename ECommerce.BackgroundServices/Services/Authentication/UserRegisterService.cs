using ECommerce.Core.RabbitMqMessageBroker;
using ECommerce.Mail.Abstract;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ECommerce.BackgroundServices.Services.Authentication
{
    public class UserRegisterService : BackgroundService
    {
        private readonly IMailService _mailService;
        EventingBasicConsumer _consume;
        IModel _channel;
        public UserRegisterService(IConfiguration configuration, IMailService mailService)
        {
            RabbitMqMessageConsumer consumer = new RabbitMqMessageConsumer(configuration);
            _channel = consumer.Connect();
            _consume = consumer.ConsumerMessageConfigurations("E.Commerce.Users.Registered", "E_Commerce_Users", ExchangeType.Direct, "E.Commerce.Users.Registered.Key");
            _mailService = mailService;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //_consume.Received += (sender, e) =>
            //{
            //    string userName = Encoding.UTF8.GetString(e.Body.Span);
            //    Console.WriteLine(userName);
            //    _channel.BasicAck(e.DeliveryTag, multiple: false);
            //    _mailService.SendMail("furat__@hotmail.com", "ECommerce", $"{userName}, Welcome to ECommerce!" +
            //        $"We are happy to see you among us!");
            //};

            return Task.CompletedTask;
        }
    }
}
