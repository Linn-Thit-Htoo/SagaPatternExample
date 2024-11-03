using MediatR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SagaPatternExample.Db.AppDbContextModels;
using SagaPatternExample.OrderServiceApi.Behaviors;
using SagaPatternExample.OrderServiceApi.Config;
using SagaPatternExample.OrderServiceApi.Extensions;
using SagaPatternExample.OrderServiceApi.Models;
using SagaPatternExample.Utils;
using System.Text;

namespace SagaPatternExample.OrderServiceApi.Features.Order.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderModel>>
{
    internal readonly AppDbContext _context;
    internal const string ExchangeName = "DirectExchange";
    internal const string RoutingKey = "order_direct";
    private readonly RabbitMqConfiguration _rabbitConfig;
    private const string QueueName = "OrderQueue";

    public CreateOrderCommandHandler(AppDbContext context, IConfiguration config)
    {
        _rabbitConfig = config.GetSection("RabbitMQ").Get<RabbitMqConfiguration>()!;
        _context = context;
    }

    public async Task<Result<OrderModel>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var orderEntity = request.ToEntity();
        await _context.TbOrders.AddAsync(orderEntity, cancellationToken);
        foreach (var item in request.OrderDetails)
        {
            await _context.TbOrderDetails.AddAsync(item.ToEntity(orderEntity.InvoiceNo), cancellationToken);
        }
        await _context.SaveChangesAsync(cancellationToken);

        var orderCreatedSuccessEvent = new OrderCreatedEvent()
        {
            InvoiceNo = orderEntity.InvoiceNo,
            OrderDetails = request.OrderDetails
        };
        PublishOrderCreatedMessage(orderCreatedSuccessEvent);

        return Result<OrderModel>.Success();
    }
    private IConnection CreateChannel()
    {
        ConnectionFactory connectionFactory = new ConnectionFactory
        {
            //HostName = _rabbitConfig.HostName,
            UserName = _rabbitConfig.Username,
            Password = _rabbitConfig.Password,
            HostName = "localhost",
            VirtualHost = "/",
            //ClientProperties = { ["IP"] = myIP }
        };

        connectionFactory.AutomaticRecoveryEnabled = true;
        connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(5);
        connectionFactory.RequestedHeartbeat = TimeSpan.FromMinutes(5);
        connectionFactory.DispatchConsumersAsync = true;

        return connectionFactory.CreateConnection();
    }

    private void PublishOrderCreatedMessage(OrderCreatedEvent orderCreatedEvent)
    {
        IConnection _connection = this.CreateChannel();
        using var channel = _connection.CreateModel();

        channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct, durable: true);

        channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: RoutingKey);
        
        var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(orderCreatedEvent));

        channel.BasicPublish(
            exchange: ExchangeName,
            routingKey: RoutingKey,
            basicProperties: null,
            body: messageBody);
    }
}
