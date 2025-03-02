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
        public string? Author { get; set; }
        public string? Content { get; set; }
        public int? Privacy { get; set; }
        public int? Views { get; set; }
    }

    public class PostDetailVM
    {
        public string? PostId { get; set; }
        public string? Title { get; set; }
        public string? Thumbnail { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Author { get; set; }
        public string? Username { get; set; }
        public string? Avatar { get; set; }
        public string? Content { get; set; }
        public int? Views { get; set; }
        public int? Privacy { get; set; }
        public int? Likes { get; set; }
        public int? Comments { get; set; }
        public string? Tags { get; set; }
    }

    public class InsertUpdatePost
    {
        public string? PostId { get; set; } = null;
        [StringLength(15000, MinimumLength = 5, ErrorMessage = "Content length too long.")]
        public string? Content { get; set; }
        [Required(ErrorMessage = "Tiêu đề không được để trống.")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Tiêu đề có ít nhất 10 kí tự và tối đa 200 kí tự.")]
        public string Title { get; set; } = null!;
        public int? Privacy { get; set; } = 0!;
        public string? Tags { get; set; }
        [Required(ErrorMessage = "Vui lòng ghi tên tác giả.")]
        public string? Author { get; set; }
        public bool? IsComment { get; set; } = true;
        public bool? PinTop { get; set; } = false;
        public DateTime? CreatedAt { get; set; }
        [Required(ErrorMessage = "Vui lòng thêm hình nhỏ (Thumbnail).")]
        public IFormFile? Thumbnail { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn thể loại hàng.")]
        public int CategoryId { get; set; }
    }
}
