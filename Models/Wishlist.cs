using System;
using System.Collections.Generic;

namespace ECommerce.Models;

public partial class Wishlist
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int ProductId { get; set; }

    public DateTime DateAdded { get; set; }

    public virtual Product Product { get; set; } = null!;
}
