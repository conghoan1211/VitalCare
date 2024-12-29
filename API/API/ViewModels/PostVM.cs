namespace API.ViewModels
{
    public class PostVM
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string? Username { get; set; }
        public string? Avatar { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? VideoUrl { get; set; }
        public string? Caption { get; set; }
        public int? Likes { get; set; }
        public int? Comments { get; set; }
        public int? Shares { get; set; }
        public int? Views { get; set; }
        public ulong? IsComment { get; set; }
        public ulong? IsPined { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }

}
