
using RabbitMQ.Client;
using SagaPatternExample.StockServiceApi.Config;

namespace SagaPatternExample.StockServiceApi.Services
{
    public class RabbitMQService : BackgroundService
    {
        internal readonly RabbitMqConfiguration _rabbitConfig;
        internal readonly IServiceScopeFactory _scopeFactory;

        public RabbitMQService(IConfiguration config, IServiceScopeFactory scopeFactory)
        {
            _rabbitConfig = config.GetSection("RabbitMQ").Get<RabbitMqConfiguration>()!;
            _scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
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
    }
}
