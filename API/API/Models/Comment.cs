using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Comment
{
    public string CommentId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string? Content { get; set; }

    public int? Likes { get; set; }

    public int? IsHide { get; set; }

    public bool? IsPinTop { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int TypeObject { get; set; }

    public string EntityId { get; set; } = null!;

    public virtual Post Entity { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
