using Microsoft.EntityFrameworkCore;
using SagaPatternExample.Db.AppDbContextModels;
using SagaPatternExample.StockServiceApi.Models;
using SagaPatternExample.Utils;

namespace SagaPatternExample.StockServiceApi.Services
{
    public class StockService : IStockService
    {
        internal readonly AppDbContext _context;

        public StockService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<StockModel>> ProcessStockAsync(OrderProductRequestModel requestModel, CancellationToken cs)
        {
            Result<StockModel> result;
            try
            {
                foreach (var item in requestModel.OrderDetails)
                {
                    var stock = await _context.TbStockEntries
                        .Where(x => x.ProductId == item.ProductId)
                        .FirstOrDefaultAsync(cs);
                    ArgumentNullException.ThrowIfNull(stock);

                    if (item.Qty > stock.Stock)
                    {
                        // fail stock event
                        result = Result<StockModel>.Fail("Insufficient Stock.");
                        goto result;
                    }

                    stock.Stock -= item.Qty;
                    _context.TbStockEntries.Update(stock);
                }

                await _context.SaveChangesAsync(cs);
                result = Result<StockModel>.Success();
            }
            catch (Exception ex)
            {
                result = Result<StockModel>.Fail(ex);
            }

        result:
            return result;
        }
    }
}
