using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Product
{
    public string ProductId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public string? ProductUrl { get; set; }

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public int? Sold { get; set; }

    public int? Stock { get; set; }

    public int? Status { get; set; }

    public double? Rating { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int CurrentPrice { get; set; }

    public int? NewPrice { get; set; }

    public string? CreateUser { get; set; }

    public string? UpdateUser { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
