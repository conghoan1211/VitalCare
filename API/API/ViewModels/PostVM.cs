using ExpressiveAnnotations.Attributes;
using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class PostVM
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
        public string Title { get; set; } = null!;
        public string Thumbnail { get; set; } = null!;
        public int CategoryId { get; set; }
        public string? CategoryName{ get; set; }
    }

    public class PostListVM
    {
        public string? PostId { get; set; }
        public string? Title {  get; set; }
        public string? Thumbnail {  get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class InsertUpdatePost
    {
        public string? PostId { get; set; }
        public string UserId { get; set; } = null!;
        [StringLength(3000, MinimumLength = 5, ErrorMessage = "Content length too long.")]
        public string? Content { get; set; }
        [Required(ErrorMessage = "Title cannot be empty or null.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Title length required from 5 to 100 chars.")]
        public string Title { get; set; } = null!;
        public string? VideoUrl { get; set; }
        public int? Privacy { get; set; } = 0!;
        public string? Tags { get; set; }
        [Required(ErrorMessage = "Vui lòng ghi tên tác giả.")]
        public string? Author { get; set; }
        public bool? IsComment { get; set; } = true;
        public bool? PinTop { get; set; } = false;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreateUser { get; set; }
        public string? UpdateUser { get; set; }
        public IFormFile? Thumbnail { get; set; }
        public int CategoryId { get; set; }
    }
}
