using SagaPatternExample.StockServiceApi.Models;

namespace SagaPatternExample.StockServiceApi.Services
{
    public interface IStockService
    {
        Task ProcessStockAsync(OrderProductRequestModel requestModel, CancellationToken cs);
    }
}
