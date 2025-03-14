using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class InsertOrderVM
    {
        [Required(ErrorMessage = "UserID cannot null!")]
        public string? UserID { get; set; }

        [Required(ErrorMessage = "Tên khách hàng không được để trống")]
        [StringLength(30, ErrorMessage = "Tên khách hàng không được vượt quá 30 ký tự")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^\d{10,10}$", ErrorMessage = "Số điện thoại là chuỗi 10 ký tự chữ số")]
        public string? Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [StringLength(100, ErrorMessage = "Email quá dài")]
        [EmailAddress(ErrorMessage = "Định dạng Email không đúng")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn tỉnh/thành phố.")]
        public string? ProvinceName { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn địa chỉ huyện.")]
        public string? DistrictName { get; set; }
        [Required(ErrorMessage = "Vui lòng điền địa chỉ chi tiết để nhận hàng.")]
        public string? SpecificAddress { get; set; }
        public string? Note { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán.")]
        public int PaymentMethod { get; set; }
        public int Total { get; set; }
        public int ShipPrice { get; set; }
        public string? ProvinceId { get; set; }
        public string? DistrictId { get; set; }
        public DateTime? OrderDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Danh sách sản phẩm không được để trống.")]
        public List<OrderDetailVM> Products { get; set; } = new List<OrderDetailVM>();
    }

    // Model cho sản phẩm trong đơn hàng
    public class OrderDetailVM
    {
        [Required(ErrorMessage = "ProductID không được để trống.")]
        public string? ProductID { get; set; }
        [Required(ErrorMessage = "Tiêu đề không được để trống.")]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Loại hàng không được để trống.")]
        public string? CategoryName { get; set; }
        [Required(ErrorMessage = "Giá hiện tại không được để trống.")]
        public int CurrentPrice { get; set; }
        [Required(ErrorMessage = "Giá mới không được để trống.")]
        public int? NewPrice { get; set; }
        [Required(ErrorMessage = "Url ảnh không được để trống.")]
        public string? ProductUrl { get; set; }
        [Required(ErrorMessage = "Số lượng không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }

    }

    public class OrderVM
    {
        public string? OrderId { get; set; }
        public string? UserID { get; set; }
        public string? UserName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? SpecificAddress { get; set; }
        public string? Note { get; set; }
        public int Status { get; set; }
        public int PaymentMethod { get; set; }
        public int Total { get; set; }
        public int ShipPrice { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? OrderDate { get; set; }
        public List<OrderDetailVM> Products { get; set; } = new List<OrderDetailVM>();
    }

    public class OrderProfileVM
    {
        public string? OrderId { get; set; }
        public string? Note { get; set; }
        public int Status { get; set; }
        public int Total { get; set; }
        public int ShipPrice { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<OrderDetailVM> Products { get; set; } = new List<OrderDetailVM>();
    }
}
