using System;
using System.Collections.Generic;

namespace ECommerce.Models;

public partial class Order
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Aderss { get; set; }

    public string? Mobile { get; set; }

    public string? UserId { get; set; }

    public decimal? Total { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
