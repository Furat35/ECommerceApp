using ECommerce.Core.RabbitMqMessageBroker.Abstract;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ECommerce.Core.RabbitMqMessageBroker
{
    public class RabbitMqMessageConsumer : IRabbitMqMessageConsumer
    {
        ConnectionFactory _connectionFactory;
        IConnection _connection;
        IModel _channel;

        public RabbitMqMessageConsumer(IConfiguration configuration)
        {
            _connectionFactory = new ConnectionFactory()
            {
                UserName = configuration.GetValue<string>("RabbitMq:UserName"),
                Password = configuration.GetValue<string>("RabbitMq:Password"),
                HostName = configuration.GetValue<string>("RabbitMq:HostName")
            };
        }

        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();

            return _channel;
        }

        public EventingBasicConsumer ConsumerMessageConfigurations(string queue, string exchange, string exchangeType, string routingKey)
        {
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            _channel.QueueDeclare(queue, true, false, false, null);
            _channel.ExchangeDeclare(
                exchange: exchange,
                type: exchangeType,
                durable: true);
            _channel.QueueBind(queue: queue, exchange: exchange, routingKey: routingKey);
            var consumer = new EventingBasicConsumer(_channel);
            _channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);

            return consumer;
        }
    }
}
