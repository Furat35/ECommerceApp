using ECommerce.Core.BackgroundServices.DTOs;
using ECommerce.Core.RabbitMqMessageBroker;
using ECommerce.Core.RabbitMqMessageBroker.Abstract;
using ECommerce.Service.Abstract;
using ECommerce.Service.Extensions;
using Microsoft.AspNetCore.Http;
using RabbitMQ.Client;

namespace ECommerce.Service.Concrete
{
    public class UserMessageService : IUserMessageService
    {
        private static IRabbitMqMessagePublisher _messagePublisherService;
        private readonly IHttpContextAccessor _httpContext;

        public UserMessageService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        static UserMessageService()
        {
            _messagePublisherService = new RabbitMqMessagePublisher();
            _messagePublisherService.Connect("E.Commerce.UserMessages.Messages", "E_Commerce_UserMessages", ExchangeType.Direct, "E.Commerce.UserMessages.Messages.Key");
        }

        public async Task SendToUsersWithEmail(List<string> to, string subject, string content)
        {
            _messagePublisherService.PublishMessage(new UserMessage
            {
                From = _httpContext.HttpContext.User.GetUserEmail() ?? "",
                To = to,
                Content = content,
                Subject = subject
            });
        }

        public async Task SentToAllUsers(string to, string subject, string content)
        {
            _messagePublisherService.PublishMessage(new UserMessage
            {
                From = _httpContext.HttpContext.User.GetUserEmail() ?? "",
                To = null,
                Content = content,
                Subject = subject
            });
        }
    }
}
