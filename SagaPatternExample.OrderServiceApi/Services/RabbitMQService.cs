using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SagaPatternExample.OrderServiceApi.Behaviors;
using SagaPatternExample.OrderServiceApi.Config;

namespace SagaPatternExample.OrderServiceApi.Services;

public class RabbitMQService : BackgroundService
{
    internal readonly RabbitMqConfiguration _rabbitConfig;
    internal readonly IServiceScopeFactory _scopeFactory;

    public RabbitMQService(IServiceScopeFactory scopeFactory, IConfiguration config)
    {
        _scopeFactory = scopeFactory;
        _rabbitConfig = config.GetSection("RabbitMQ").Get<RabbitMqConfiguration>()!;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IConnection _connection = this.CreateChannel();
        var _channel = _connection.CreateModel();

        foreach (Queues item in _rabbitConfig.QueueList!)
        {
            _channel.ExchangeDeclare(item.Exchange, "direct", durable: true);
            _channel.QueueDeclare(
                queue: item.Queue,
                durable: true,
                exclusive: false,
                autoDelete: false
            );
            _channel.QueueBind(item.Queue, item.Exchange, item.RoutingKey, null);
            _channel.BasicQos(0, 1, false);
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());

                if (ea.RoutingKey.Equals("stock_direct"))
                {
                    StockReductionFailEvent _event =
                        JsonConvert.DeserializeObject<StockReductionFailEvent>(content)!;
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IOrderService>();

                    await service.RollbackOrderAsync(_event.InvoiceNo, stoppingToken);
                }
            };
            _channel.BasicConsume(item.Queue, false, consumer);
        }
        await Task.CompletedTask;
    }

    private IConnection CreateChannel()
    {
        ConnectionFactory connectionFactory =
            new()
            {
                HostName = _rabbitConfig.HostName,
                UserName = _rabbitConfig.Username,
                Password = _rabbitConfig.Password,
                VirtualHost = "/",
            };

        connectionFactory.AutomaticRecoveryEnabled = true;
        connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(5);
        connectionFactory.RequestedHeartbeat = TimeSpan.FromMinutes(5);
        connectionFactory.DispatchConsumersAsync = true;

        return connectionFactory.CreateConnection();
    }
}
