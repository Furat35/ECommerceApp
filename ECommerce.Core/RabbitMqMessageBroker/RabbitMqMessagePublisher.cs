using ECommerce.Core.RabbitMqMessageBroker.Abstract;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ECommerce.Core.RabbitMqMessageBroker
{
    public class RabbitMqMessagePublisher : IRabbitMqMessagePublisher
    {
        ConnectionFactory _connectionFactory;
        IConnection _connection;
        IModel _channel;
        public string _exchangeType;
        public string _exchangeName;
        public string _queueName;
        public string _routingKey;

        public RabbitMqMessagePublisher(IConfiguration configuration)
        {
            _connectionFactory = new ConnectionFactory()
            {
                UserName = configuration.GetValue<string>("RabbitMq:UserName"),
                Password = configuration.GetValue<string>("RabbitMq:Password"),
                HostName = configuration.GetValue<string>("RabbitMq:HostName")
            };
        }

        public RabbitMqMessagePublisher()
        {
            _connectionFactory = new ConnectionFactory()
            {
                UserName = "guest",
                Password = "guest",
                HostName = "localhost"
            };
        }

        public IModel Connect(string queue, string exchange, string exchangeType, string routingKey)
        {
            _exchangeType = exchangeType;
            _queueName = queue;
            _routingKey = routingKey;
            _exchangeName = exchange;
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            _channel.ExchangeDeclare(
                exchange: exchange,
                type: exchangeType,
                durable: true);
            //_channel.QueueDeclare(queue, true, false, false, null);
            //_channel.QueueBind(
            //    queue: queue,
            //    exchange: exchange,
            //    routingKey: routingKey);

            return _channel;
        }

        public void PublishMessage(object message)
        {
            _channel.BasicPublish(
               exchange: _exchangeName,
               routingKey: _routingKey,
               body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _channel = null;
        }
    }
}
