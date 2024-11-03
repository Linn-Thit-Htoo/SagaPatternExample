using Microsoft.EntityFrameworkCore;
using SagaPatternExample.Db.AppDbContextModels;
using SagaPatternExample.OrderServiceApi.Config;
using SagaPatternExample.OrderServiceApi.Services;

namespace SagaPatternExample.OrderServiceApi.Extensions;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlServer(config.GetConnectionString("DbConnection"));
        }, ServiceLifetime.Transient, ServiceLifetime.Transient);

        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(typeof(DependencyInjectionExtension).Assembly);
        });

        services.AddScoped<IOrderService, OrderService>();

        services.AddHttpContextAccessor();
        services.Configure<RabbitMqConfiguration>(config.GetSection("RabbitMQ"));
        services.AddHealthChecks();

        services.AddHostedService<RabbitMQService>();

        return services;
    }
}
