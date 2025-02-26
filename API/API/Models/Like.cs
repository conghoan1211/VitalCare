using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Like
{
    public string LikeId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public int TypeObject { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string EntityId { get; set; } = null!;

    public virtual Post Entity { get; set; } = null!;

    public virtual Video EntityNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
