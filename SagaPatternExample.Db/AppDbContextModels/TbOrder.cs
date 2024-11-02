using System;
using System.Collections.Generic;

namespace SagaPatternExample.Db.AppDbContextModels;

public partial class TbOrder
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public decimal TotalAmount { get; set; }

    public string InvoiceNo { get; set; } = null!;

    public virtual TbUser User { get; set; } = null!;
}
