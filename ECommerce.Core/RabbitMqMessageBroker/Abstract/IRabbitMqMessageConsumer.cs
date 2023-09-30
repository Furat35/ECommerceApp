using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ECommerce.Core.RabbitMqMessageBroker.Abstract
{
    public interface IRabbitMqMessageConsumer
    {
        IModel Connect();
        EventingBasicConsumer ConsumerMessageConfigurations(string queue, string exchange, string exchangeType, string routingKey);
    }
}
