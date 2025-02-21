using System;
using System.Collections.Generic;

namespace API.Models;

public partial class UserDailyUsage
{
    public string UsageId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public DateOnly UsageDate { get; set; }

    public int? QuestionCount { get; set; }

    public int? MaxQuestionsPerDay { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
