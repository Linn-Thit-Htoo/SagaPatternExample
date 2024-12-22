namespace SagaPatternExample.OrderServiceApi.Events
{
    public class OrderSubmittedEvent
    {
        public Guid InvoiceNo { get; set; }
        public List<CreateOrderDetialRequestDTO> OrderDetails { get; set; }
    }

    public class StockReductionEvent
    {
        public Guid Invoice { get; set; }
    }

    public class CreateOrderDetialRequestDTO
    {
        public int ProductId { get; set; }

        public int Qty { get; set; }

        public decimal Price { get; set; }

        public decimal Subtotal { get; set; }
    }
}
