using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Category
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int TypeObject { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
}
