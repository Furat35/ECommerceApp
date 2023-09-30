using ECommerce.Core.BackgroundServices.DTOs;
using ECommerce.Core.RabbitMqMessageBroker;
using ECommerce.Entity.Entities.IdentityFrameworkEntities;
using ECommerce.Mail.Abstract;
using Microsoft.AspNetCore.Identity;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ECommerce.BackgroundServices.Services.UserMessages
{
    public class UserMessageService : BackgroundService
    {
        private readonly IMailService _mailService;
        public UserManager<AppUser> _userManager;
        EventingBasicConsumer _consume;
        IModel _channel;

        public UserMessageService(IConfiguration configuration, IMailService mailService, UserManager<AppUser> userManager)
        {
            RabbitMqMessageConsumer consumer = new RabbitMqMessageConsumer(configuration);
            _channel = consumer.Connect();
            _consume = consumer.ConsumerMessageConfigurations("E.Commerce.UserMessages.Messages", "E_Commerce_UserMessages", ExchangeType.Direct, "E.Commerce.UserMessages.Messages.Key");
            _mailService = mailService;
            _userManager = userManager;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consume.Received += (sender, e) =>
            {
                var consumeData = Encoding.UTF8.GetString(e.Body.Span);
                var orderDetail = JsonSerializer.Deserialize<UserMessage>(consumeData);

                if (orderDetail.To != null)
                    foreach (string to in orderDetail.To)
                    {
                        _channel.BasicAck(e.DeliveryTag, multiple: false);
                        _mailService.SendMail($"{to}", orderDetail.Subject, orderDetail.Content);
                    }
                else
                {
                    List<string> emails = _userManager.Users.Where(_ => _.DeletedDate == null).Select(_ =>
                        _.Email
                    ).ToList();
                    foreach (string to in emails)
                    {
                        _channel.BasicAck(e.DeliveryTag, multiple: false);
                        _mailService.SendMail($"{to}", orderDetail.Subject, orderDetail.Content);
                    }
                }
            };

            return Task.CompletedTask;
        }
    }
}
