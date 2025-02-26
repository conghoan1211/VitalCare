using System;
using System.Collections.Generic;

namespace API.Models;

public partial class VideoComment
{
    public string CommentId { get; set; } = null!;

    public string VideoId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? ParentCommentId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<VideoComment> InverseParentComment { get; set; } = new List<VideoComment>();

    public virtual VideoComment? ParentComment { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Video Video { get; set; } = null!;
}
