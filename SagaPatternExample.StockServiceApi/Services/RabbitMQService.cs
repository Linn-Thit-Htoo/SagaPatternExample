
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SagaPatternExample.StockServiceApi.Behaviors;
using SagaPatternExample.StockServiceApi.Config;
using SagaPatternExample.StockServiceApi.Models;
using System.Text;

namespace SagaPatternExample.StockServiceApi.Services
{
    public class RabbitMQService : BackgroundService
    {
        internal readonly RabbitMqConfiguration _rabbitConfig;
        internal readonly IServiceScopeFactory _scopeFactory;
        internal const string ExchangeName = "DirectExchange";
        internal const string RoutingKey = "order_direct";
        private const string QueueName = "OrderQueue";

        public RabbitMQService(IConfiguration config, IServiceScopeFactory scopeFactory)
        {
            _rabbitConfig = config.GetSection("RabbitMQ").Get<RabbitMqConfiguration>()!;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IConnection _connection = this.CreateChannel();
            var _channel = _connection.CreateModel();

            foreach (Queues item in _rabbitConfig.QueueList!)
            {
                _channel.ExchangeDeclare(item.Exchange, "direct", durable: true);
                _channel.QueueDeclare(queue: item.Queue, durable: true, exclusive: false, autoDelete: false);
                _channel.QueueBind(item.Queue, item.Exchange, item.RoutingKey, null);
                _channel.BasicQos(0, 1, false);
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += async (ch, ea) =>
                {
                    var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                    if (ea.RoutingKey.Equals("order_direct"))
                    {
                        OrderProductRequestModel requestModel = JsonConvert.DeserializeObject<OrderProductRequestModel>(content)!;
                        using var scope = _scopeFactory.CreateScope();
                        var stockService = scope.ServiceProvider.GetRequiredService<IStockService>();

                        var result = await stockService.ProcessStockAsync(requestModel, stoppingToken);
                        if (!result.IsSuccess)
                        {
                            PublishStockFailEvent(requestModel.InvoiceNo);
                        }
                    }
                };
                _channel.BasicConsume(item.Queue, false, consumer);
            }

            await Task.CompletedTask;
        }

        private IConnection CreateChannel()
        {
            ConnectionFactory connectionFactory = new()
            {
                HostName = _rabbitConfig.HostName,
                UserName = _rabbitConfig.Username,
                Password = _rabbitConfig.Password,
                VirtualHost = "/"
            };

            connectionFactory.AutomaticRecoveryEnabled = true;
            connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(5);
            connectionFactory.RequestedHeartbeat = TimeSpan.FromMinutes(5);
            connectionFactory.DispatchConsumersAsync = true;

            return connectionFactory.CreateConnection();
        }

        private void PublishStockFailEvent(string invoiceNo)
        {
            IConnection connection = CreateChannel();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct, durable: true);

            channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: RoutingKey);

            var stockFailEvent = new StockReductionFailEvent() { InvoiceNo = invoiceNo };
            var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(stockFailEvent));

            channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: RoutingKey,
                basicProperties: null,
                body: messageBody);
        }
    }
}
