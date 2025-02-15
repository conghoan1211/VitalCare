using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class CategoryVM
    {
        public int Id { get; set; } 
        public string? Name { get; set; } 
        public int TypeObject { get; set; }
        public int Number { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }

    public class InsertUpdateCategory
    {
        public int? Id { get; set; } = null;
        [Required(ErrorMessage = "Tiêu đề thể loại hàng không được bỏ trống!")]
        [StringLength(25, MinimumLength = 5, ErrorMessage = "Tiêu đề có độ dài bắt buộc từ 5 đến 25 kí tự!")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn kiểu đối tượng!")]
        public int TypeObject { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
