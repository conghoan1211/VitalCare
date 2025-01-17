using API.Models;

namespace API.ViewModels
{
    public class LoginResult
    {
        public string? Token { get; set; }
        public UserVM? User { get; set; }
    }

    public class LoginGoogleResult
    {
        public string? Token { get; set; }
        public bool? HasPassword { get; set; }
        public UserVM? User { get; set; }
    }

    public class UserVM
    {
        public string UserId { get; set; } = null!;
        public string? Username { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Avatar { get; set; }
        public string? Address { get; set; }
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
    }

    public class UserListVM
    {
        public string UserId { get; set; } = null!;
        public string? Username { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public string? Address { get; set; }
        public int? Sex { get; set; }
        public DateTime? Dob { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsVerified { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? CreateUser { get; set; }
        public string? UpdateUser { get; set; }
        public int? Status { get; set; }
        public DateTime? BlockUntil { get; set; }
    }

}
