using Microsoft.EntityFrameworkCore;
using SagaPatternExample.Db.AppDbContextModels;

namespace SagaPatternExample.OrderServiceApi.Services;

public class OrderService : IOrderService
{
    internal readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task RollbackOrderAsync(string invoice, CancellationToken cs)
    {
        var item = await _context.TbOrders.FirstOrDefaultAsync(
            x => x.InvoiceNo == invoice,
            cancellationToken: cs
        );
        ArgumentNullException.ThrowIfNull(item, nameof(item));

        var lst = await _context.TbOrderDetails.Where(x => x.InvoiceNo == invoice).ToListAsync(cs);
        ArgumentNullException.ThrowIfNull(lst, nameof(lst));

        _context.TbOrders.Remove(item);
        _context.TbOrderDetails.RemoveRange(lst);

        await _context.SaveChangesAsync(cs);
    }
}
