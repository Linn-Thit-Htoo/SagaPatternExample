namespace SagaPatternExample.OrderServiceApi.Services;

public interface IOrderService
{
    Task RollbackOrderAsync(string invoice, CancellationToken cs);
}
