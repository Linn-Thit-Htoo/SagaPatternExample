using System;
using System.Collections.Generic;

namespace SagaPatternExample.Db.AppDbContextModels;

public partial class TbProduct
{
    public int ProductId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<TbOrderDetail> TbOrderDetails { get; set; } = new List<TbOrderDetail>();

    public virtual ICollection<TbStockEntry> TbStockEntries { get; set; } = new List<TbStockEntry>();
}
