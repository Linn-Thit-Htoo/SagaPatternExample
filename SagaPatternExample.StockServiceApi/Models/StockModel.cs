namespace SagaPatternExample.StockServiceApi.Models
{
    public class OrderProductRequestModel
    {
        public List<ProductRequestModel> Products { get; set; }
    }

    public class ProductRequestModel
    {
        public int ProductId { get; set; }
        public int Qty { get; set; }
    }
}
