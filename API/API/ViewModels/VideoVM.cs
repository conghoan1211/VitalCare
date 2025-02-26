using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class VideoListVM
    {
        public string VideoId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string VideoUrl { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string? Duration { get; set; }
        public string? Views { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class VideoDetailVM
    {
        public string VideoId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string VideoUrl { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string? ThumbnailUrl { get; set; }
        public string? Duration { get; set; }
        public string? CategoryId { get; set; }
        public string? Views { get; set; }
        public int? Likes { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class InsertUpdateVideoVM
    {
        public string? VideoId { get; set; }
        [Required(ErrorMessage = "Tiêu đề không được để trống.")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Tiêu đề có ít nhất 10 kí tự và tối đa 200 kí tự.")]
        public string? Title { get; set; }
        [MaxLength(1000, ErrorMessage = "Mô tả không quá 1000 kí tự!")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập đường dẫn video.")]
        public string? VideoUrl { get; set; } 
        [Required(ErrorMessage = "Vui lòng ghi tên tác giả.")]
        public string? Author { get; set; } 
        public string? ThumbnailUrl { get; set; }
        [Required(ErrorMessage = "Thời lượng video không được để trống.")]
        public string? Duration { get; set; }
        public int? CategoryId { get; set; }  // update type object string to int in db
        public DateTime? CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
