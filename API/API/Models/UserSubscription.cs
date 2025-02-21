using System;
using System.Collections.Generic;

namespace API.Models;

public partial class UserSubscription
{
    public string SubscriptionId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    public int? MaxQuestions { get; set; }

    public int? UsedQuestions { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
