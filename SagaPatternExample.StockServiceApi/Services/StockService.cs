using Microsoft.EntityFrameworkCore;
using SagaPatternExample.Db.AppDbContextModels;
using SagaPatternExample.StockServiceApi.Models;

namespace SagaPatternExample.StockServiceApi.Services
{
    public class StockService : IStockService
    {
        internal readonly AppDbContext _context;

        public StockService(AppDbContext context)
        {
            _context = context;
        }

        public async Task ProcessStockAsync(OrderProductRequestModel requestModel, CancellationToken cs)
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
                }

                else
                {
                    stock.Stock -= item.Qty;
                    _context.TbStockEntries.Update(stock);
                    await _context.SaveChangesAsync(cs);
                }
            }
        }
    }
}
