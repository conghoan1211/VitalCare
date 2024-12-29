﻿using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Post
{
    public string PostId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string? Content { get; set; }

    public string? VideoUrl { get; set; }

    public int? Privacy { get; set; }

    public string? Tags { get; set; }

    public string? Author { get; set; }

    public int? Likes { get; set; }

    public int? Comments { get; set; }

    public int? Shares { get; set; }

    public int? Views { get; set; }

    public bool? IsComment { get; set; }

    public bool? PinTop { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreateUser { get; set; }

    public string? UpdateUser { get; set; }

    public virtual User User { get; set; } = null!;
}