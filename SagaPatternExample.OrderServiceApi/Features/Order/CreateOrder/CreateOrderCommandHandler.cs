using MediatR;
using SagaPatternExample.Db.AppDbContextModels;
using SagaPatternExample.OrderServiceApi.Extensions;
using SagaPatternExample.OrderServiceApi.Models;
using SagaPatternExample.Utils;

namespace SagaPatternExample.OrderServiceApi.Features.Order.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderModel>>
    {
        internal readonly AppDbContext _context;

        public CreateOrderCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<OrderModel>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = request.ToEntity();
            await _context.TbOrders.AddAsync(orderEntity, cancellationToken);
            foreach (var item in request.OrderDetils)
            {
                await _context.TbOrderDetails.AddAsync(item.ToEntity(orderEntity.InvoiceNo), cancellationToken);
            }
            await _context.SaveChangesAsync(cancellationToken);

            return Result<OrderModel>.Success();
        }
    }
}
