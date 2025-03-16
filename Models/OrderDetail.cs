using System;
using System.Collections.Generic;

namespace ECommerce.Models;

public partial class OrderDetail
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public decimal? Price { get; set; }

    public int? Qty { get; set; }

    public decimal? Total { get; set; }

    public int? OrderId { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
