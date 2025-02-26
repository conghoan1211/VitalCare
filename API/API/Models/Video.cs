using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Video
{
    public string VideoId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string VideoUrl { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string? ThumbnailUrl { get; set; }

    public string? Duration { get; set; }

    public int? CategoryId { get; set; }

    public int? Views { get; set; }

    public int? Likes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreateUser { get; set; }

    public string? UpdateUser { get; set; }

    public bool? IsActive { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Like> LikesNavigation { get; set; } = new List<Like>();

    public virtual ICollection<VideoComment> VideoComments { get; set; } = new List<VideoComment>();
}
