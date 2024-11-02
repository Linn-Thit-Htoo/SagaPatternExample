using System;
using System.Collections.Generic;

namespace SagaPatternExample.Db.AppDbContextModels;

public partial class TbOrderDetail
{
    public int Id { get; set; }

    public string InvoiceNo { get; set; } = null!;

    public int ProductId { get; set; }

    public int Qty { get; set; }

    public decimal Price { get; set; }

    public decimal Subtotal { get; set; }

    public virtual TbProduct Product { get; set; } = null!;
}
