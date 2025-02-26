using System;
using System.Collections.Generic;

namespace API.Models;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string? Username { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Avatar { get; set; }

    public int? RoleId { get; set; }

    public string? GoogleId { get; set; }

    public int? Sex { get; set; }

    public DateTime? Dob { get; set; }

    public string? Bio { get; set; }

    public bool? IsDisable { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsVerified { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public string? CreateUser { get; set; }

    public string? UpdateUser { get; set; }

    public int? Status { get; set; }

    public DateTime? BlockUntil { get; set; }

    public DateTime? LastLogin { get; set; }

    public string? LastLoginIp { get; set; }

    public string? Address { get; set; }

    public string? ProvinceId { get; set; }

    public string? DistrictId { get; set; }

    public string? DistrictName { get; set; }

    public string? ProvinceName { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? ExpiryDateToken { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<UserDailyUsage> UserDailyUsages { get; set; } = new List<UserDailyUsage>();

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();

    public virtual ICollection<VideoComment> VideoComments { get; set; } = new List<VideoComment>();
}
