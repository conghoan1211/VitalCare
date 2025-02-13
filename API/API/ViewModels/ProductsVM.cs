using ExpressiveAnnotations.Attributes;
using Org.BouncyCastle.Utilities.Encoders;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using API.Helper;
using System.Text.Json.Serialization;

namespace API.ViewModels
{
    public class ProductsVM 
    {
        public string ProductId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string ProductUrl { get; set; } = null!;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int CurrentPrice { get; set; }
        public int? NewPrice { get; set; }
        public int? Sold { get; set; }
        public int? Stock { get; set; }
        public int? Status { get; set; }
        public double? Rating { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class InsertUpdateProductVM
    {
        public string? ProductId { get; set; } 
        [Required(ErrorMessage = "Tiêu đề sản phẩm không được bỏ trống!")]
        [StringLength(100, MinimumLength = 15, ErrorMessage = "Tiêu đề có độ dài bắt buộc từ 15 đến 100")]
        public string Title { get; set; } = null!;
        [Required(ErrorMessage = "Cần ít nhất 1 ảnh để hiển thị sản phẩm!")]
        public List<IFormFile>? ImageUrl { get; set; }
        public string? ProductUrl { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm!")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập giá tiền sản phẩm!")]
        [AssertThat("CurrentPrice > 0", ErrorMessage = "Giá sản phẩm cần lớn hơn hoặc bằng 0.")]
        public int CurrentPrice { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập giá tiền mới sản phẩm!")]
        [AssertThat("NewPrice > 0", ErrorMessage = "Giá sản phẩm cần lớn hơn hoặc bằng 0.")]
        public int? NewPrice { get; set; }
        [DefaultValue(0)] public int? Stock { get; set; } = 0;

        [Display(Name = "Trạng thái")]
        [DefaultValue(0)] public int? Status { get; set; } = 0;               // yeu thich/ het hang/ khuyen mai/ sale...
        [DefaultValue(5)] public double? Rating { get; set; } = 5;
        [DefaultValue(true)] public bool? IsActive { get; set; } = true;
    }


    public class ProductListVM
    {
        public string? ProductId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int CurrentPrice { get; set; }
        public int NewPrice { get; set; }
        public string? ProductUrl { get; set; }
        public float Rating { get; set; }
        public int Sold { get; set; }
    }

    public class ProductDetailVM
    {
        public string? ProductId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int CurrentPrice { get; set; }
        public int NewPrice { get; set; }
        public string? ImageUrl { get; set; }
        public string? ProductUrl { get; set; }
        public string? Description { get; set; }
        public float Rating { get; set; }
        public int Sold { get; set; }
        public int Stock {  get; set; }
        public DateTime? CreatedAt { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
