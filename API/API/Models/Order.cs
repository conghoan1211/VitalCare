using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Order
{
    public string OrderId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string SpecificAddress { get; set; } = null!;

    public string Note { get; set; } = null!;

    public DateTime? OrderDate { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int Status { get; set; }

    public int PaymentMethod { get; set; }

    public int Total { get; set; }

    public int ShipPrice { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User User { get; set; } = null!;
}
