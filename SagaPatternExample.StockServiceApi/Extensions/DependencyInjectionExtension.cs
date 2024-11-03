using Microsoft.EntityFrameworkCore;
using SagaPatternExample.Db.AppDbContextModels;
using SagaPatternExample.StockServiceApi.Config;
using SagaPatternExample.StockServiceApi.Services;

namespace SagaPatternExample.StockServiceApi.Extensions;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlServer(config.GetConnectionString("DbConnection"));
        }, ServiceLifetime.Transient, ServiceLifetime.Transient);

        services.AddScoped<IStockService, StockService>();

        services.AddHttpContextAccessor();
        services.Configure<RabbitMqConfiguration>(config.GetSection("RabbitMQ"));
        services.AddHealthChecks();

        services.AddHostedService<RabbitMQService>();

        return services;
    }
}
