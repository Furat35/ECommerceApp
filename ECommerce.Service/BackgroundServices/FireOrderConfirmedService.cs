namespace ECommerce.Mail.BackgroundServices
{
    //public class FireOrderConfirmedService : BackgroundService
    //{
    //EventingBasicConsumer _consume;
    //RabbitMQ.Client.IModel _channel;
    //public FireOrderConfirmedService(IConfiguration configuration)
    //{
    //    RabbitMqMessageConsumer consumer = new RabbitMqMessageConsumer(configuration);
    //    _channel = consumer.Connect();
    //    _consume = consumer.ConsumerMessageConfigurations("E.Commerce.Orders.Completed", "E_Commerce_Orders", ExchangeType.Direct, "E.Commerce.Orders.Completed.Key");
    //}

    //private readonly EventingBasicConsumer _consumer;

    //protected override Task ExecuteAsync(CancellationToken stoppingToken)
    //{
    //    _consume.Received += (sender, e) =>
    //    {
    //        Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
    //        _channel.BasicAck(e.DeliveryTag, multiple: false);
    //    };

    //    return Task.CompletedTask;
    //}
    //}
}
