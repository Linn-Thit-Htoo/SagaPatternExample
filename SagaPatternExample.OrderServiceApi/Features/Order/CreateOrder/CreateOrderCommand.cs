using MediatR;
using SagaPatternExample.OrderServiceApi.Models;
using SagaPatternExample.Utils;

namespace SagaPatternExample.OrderServiceApi.Features.Order.CreateOrder;

public class CreateOrderCommand : IRequest<Result<OrderModel>>
{
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<CreateOrderDetialRequestDTO> OrderDetails { get; set; }
}

public class CreateOrderDetialRequestDTO
{
    public int ProductId { get; set; }

    public int Qty { get; set; }

    public decimal Price { get; set; }

    public decimal Subtotal { get; set; }
}
