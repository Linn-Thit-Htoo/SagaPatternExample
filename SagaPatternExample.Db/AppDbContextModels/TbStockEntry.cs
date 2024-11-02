using System;
using System.Collections.Generic;

namespace SagaPatternExample.Db.AppDbContextModels;

public partial class TbStockEntry
{
    public int StockId { get; set; }

    public int ProductId { get; set; }

    public int Stock { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual TbProduct Product { get; set; } = null!;
}
