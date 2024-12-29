﻿using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Product
{
    public string ProductId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int? Price { get; set; }

    public string? ImageUrl { get; set; }

    public string ProductUrl { get; set; } = null!;

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public int? Sold { get; set; }

    public int? Stock { get; set; }

    public int? Status { get; set; }

    public double? Rating { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Category Category { get; set; } = null!;
}