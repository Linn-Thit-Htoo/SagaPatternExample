using SagaPatternExample.OrderServiceApi.Features.Order.CreateOrder;

namespace SagaPatternExample.OrderServiceApi.Behaviors;

public class OrderCreatedEvent
{
    public string InvoiceNo { get; set; }
    public List<CreateOrderDetialRequestDTO> OrderDetails { get; set; }
}
