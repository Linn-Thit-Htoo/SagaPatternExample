using SagaPatternExample.StockServiceApi.Models;
using SagaPatternExample.Utils;

namespace SagaPatternExample.StockServiceApi.Services;

public interface IStockService
{
    Task<Result<StockModel>> ProcessStockAsync(
        OrderProductRequestModel requestModel,
        CancellationToken cs
    );
}
