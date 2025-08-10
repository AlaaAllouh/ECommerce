using System;
using System.Collections.Generic;

namespace ECommerce.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public string? Photo { get; set; }

    public int? Catid { get; set; }

    public string? Type { get; set; }

    public DateOnly? EntryDate { get; set; }

    public string? ReviewUrl { get; set; }

    public string? SupplierName { get; set; }

    public int? Qty { get; set; }

    public decimal? SupplierNum { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Category? Cat { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
