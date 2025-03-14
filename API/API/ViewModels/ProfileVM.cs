using API.Helper;
using ExpressiveAnnotations.Attributes;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class ProfileVM
    {
        public string? UserID { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Avatar { get; set; } = string.Empty;
        public int? RoleID { get; set; }
        public string? Bio {  get; set; } = string.Empty;
        public int? Sex { get; set; }
        public DateTime? Dob {  get; set; }
        public string? Address { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }               /* Admin choosen*/
        public void ToggleIsActive()
        {
            IsActive = !IsActive;
        }
        public bool? IsDisable { get; set; }                /*User choosen*/
        public int? Status { get; set; }                       /* trang thai hoat dong*/
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? CreateUser { get; set; }
        public string? UpdateUser { get; set; }
    }

    public class UpdateAvatarVM
    {
        [AssertThat("Image.Length <= MaxFileSize", ErrorMessage = "File size must not exceed {MaxFileSize} bytes")]
        public IFormFile? Image { get; set; }
        public long MaxFileSize => Constant.AVATAR_FILE_SIZE;
    }

    public class UpdateProfileModels
    {
        [Required(ErrorMessage = "UserID cannot null!")]
        public string? UserID { get; set; }

        [Required(ErrorMessage = "Tên hiển thị không được để trống")]
        [StringLength(30, ErrorMessage = "Tên đăng nhập không được vượt quá 30 ký tự")]
        public string? UserName { get; set; }
 
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^\d{10,10}$", ErrorMessage = "Số điện thoại là chuỗi 10 ký tự chữ số")]
        public string? Phone { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email không được để trống")]
        [StringLength(100, ErrorMessage = "Email quá dài")]
        [EmailAddress(ErrorMessage = "Định dạng Email không đúng")]
        public string? Email { get; set; }                                  // ko nên cho update email, bỏ và sửa lại fe.
        [Required(ErrorMessage = "Vui lòng chọn tỉnh/thành phố.")]
        public string? ProvinceId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn địa chỉ huyện.")]
        public string? DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public string? ProvinceName { get; set; }
        [Required(ErrorMessage = "Nhập chi tiết địa chỉ.")]
        [StringLength(150, ErrorMessage = "Địa chỉ không được vượt quá 150 ký tự")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Giới tính không được để trống")]
        [Range(0, 2, ErrorMessage = "Giới tính không hợp lệ. Vui lòng chọn: 0 (Nam), 1 (Nữ), hoặc 2 (Khác)")]
        public int? Sex { get; set; }
        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        //[BirthYearValidation(1890)]
        [AssertThat("Dob <= Now()", ErrorMessage = "Ngày sinh không vượt quá ngày hiện tại!")]
        public DateTime? Dob { get; set; }
    }

    public class BirthYearValidationAttribute : ValidationAttribute
    {
        private readonly int _minYear;
        public BirthYearValidationAttribute(int minYear)
        {
            _minYear = minYear;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var date = (DateTime)value;

            if (date.Year < _minYear)
                return new ValidationResult($"Năm sinh phải lớn hơn hoặc bằng {_minYear}!");

            return ValidationResult.Success;
        }
    }
}