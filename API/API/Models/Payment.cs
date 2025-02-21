using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Payment
{
    public string PaymentId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? Status { get; set; }

    public virtual User User { get; set; } = null!;
}
