using RabbitMQ.Client;

namespace ECommerce.Core.RabbitMqMessageBroker.Abstract
{
    public interface IRabbitMqMessagePublisher
    {
        IModel Connect(string queue, string exchange, string exchangeType, string routingKey);

        void PublishMessage(object message);
    }
}
