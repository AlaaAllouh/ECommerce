using System;
using System.Collections.Generic;

namespace ECommerce.Models;

public partial class ProductImage
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public string? Images { get; set; }

    public string? MainImage { get; set; }

    public virtual Product? Product { get; set; }
}
