using System;
using System.Collections.Generic;

namespace API.Models;

public partial class OrderDetail
{
    public string DetailId { get; set; } = null!;

    public string OrderId { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int CurrentPrice { get; set; }

    public int? NewPrice { get; set; }

    public string? ProductUrl { get; set; }

    public int Quantity { get; set; }

    public int Total { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
