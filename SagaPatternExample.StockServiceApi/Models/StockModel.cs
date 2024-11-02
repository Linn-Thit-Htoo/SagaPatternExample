namespace SagaPatternExample.StockServiceApi.Models
{
    public class OrderProductRequestModel
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

    public class ProductRequestModel
    {
        public int ProductId { get; set; }
        public int Qty { get; set; }
    }
}
