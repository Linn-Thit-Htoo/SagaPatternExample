using Microsoft.AspNetCore.Http.HttpResults;
using SagaPatternExample.Db.AppDbContextModels;
using SagaPatternExample.OrderServiceApi.Features.Order.CreateOrder;

namespace SagaPatternExample.OrderServiceApi.Extensions
{
    public static class Mapper
    {
        public static TbOrder ToEntity(this CreateOrderCommand createOrderCommand)
        {
            return new TbOrder
            {
                UserId = createOrderCommand.UserId,
                CreatedAt = DateTime.Now,
                TotalAmount = createOrderCommand.TotalAmount,
                InvoiceNo = Ulid.NewUlid().ToString()
            };
        }

        public static TbOrderDetail ToEntity(this CreateOrderDetialRequestDTO createOrderDetialRequest, string invoice)
        {
            return new TbOrderDetail
            {
                InvoiceNo = invoice,
                ProductId = createOrderDetialRequest.ProductId,
                Qty = createOrderDetialRequest.Qty,
                Price = createOrderDetialRequest.Price,
                Subtotal = createOrderDetialRequest.Subtotal
            };
        }
    }
}
