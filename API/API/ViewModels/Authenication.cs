using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string? Password { get; set; }
    }

    public class UserRegister
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập Không quá 20 ký tự")]
        public string? UserName { get; set; }
        //[Required(ErrorMessage = "Số điện thoại không được để trống")]
        //[RegularExpression(@"^\d{10,10}$", ErrorMessage = "Số điện thoại là chuỗi 10 ký tự chữ số")]
        //public string Phone { get; set; }
        [Required(ErrorMessage = "Email không được để trống")]
        [StringLength(100, ErrorMessage = "Email quá dài")]
        [EmailAddress(ErrorMessage = "Định dạng Email không đúng")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(8, ErrorMessage = "Mật khẩu chứa ít nhất 8 ký tự")]
        //[RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@#?!])[A-Za-z\d@#?!]{8,}$",
        // ErrorMessage = "Mật khẩu phải chứa ít nhất 8 ký tự, trong đó có ít nhất một chữ thường, một chữ hoa, một số và một ký tự đặc biệt.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Nhập lại mật khẩu không được để trống")]
        [Compare(nameof(Password), ErrorMessage = "Mật khẩu không khớp")]
        public string? RePassword { get; set; }
    }

    public class ForgetPassword
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [StringLength(60, ErrorMessage = "Email quá dài")]
        [EmailAddress(ErrorMessage = "Định dạng Email không đúng")]
        public string? Email { get; set; }
    }
    public class VerifyOTP
    {
        [Required(ErrorMessage = "Vui lòng nhập capcha")]
        [RegularExpression(@"^\d{6}$",
            ErrorMessage = "Sai số lượng chữ số. Hãy thử lại.")]
        public int OTP { get; set; }
        [Required(ErrorMessage = "Email không được để trống")]
        [StringLength(60, ErrorMessage = "Email quá dài")]
        [EmailAddress(ErrorMessage = "Định dạng Email không đúng")]
        public string? Email { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ExpiredDate { get; set; }
    }

    public class ResetPassword
    {
        public string? UserId { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(8, ErrorMessage = "Mật khẩu chứa ít nhất 8 ký tự")]
        //[RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@#?!])[A-Za-z\d@#?!]{8,}$",
        //    ErrorMessage = "Mật khẩu phải chứa ít nhất 8 ký tự, trong đó có ít nhất một chữ thường, một chữ hoa, một số và một ký tự đặc biệt.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Nhập lại mật khẩu không được để trống")]
        [MinLength(8, ErrorMessage = "Mật khẩu chứa ít nhất 8 ký tự")]
        [Compare(nameof(Password), ErrorMessage = "Mật khẩu không khớp")]
        public string? RePassword { get; set; }
    }

    public class ChangePassword
    {
        public string? UserId { get; set; }

        [Required(ErrorMessage = "Mật khẩu cũ không được bỏ trống")]
        public string? ExPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(8, ErrorMessage = "Mật khẩu chứa ít nhất 8 ký tự")]
        //[RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@#?!])[A-Za-z\d@#?!]{8,}$",
        //    ErrorMessage = "Mật khẩu phải chứa ít nhất 8 ký tự, trong đó có ít nhất một chữ thường, một chữ hoa, một số và một ký tự đặc biệt.")]
        public string? Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nhập lại mật khẩu không được để trống")]
        [MinLength(8, ErrorMessage = "Mật khẩu chứa ít nhất 8 ký tự")]
        [Compare(nameof(Password), ErrorMessage = "Mật khẩu không khớp")]
        public string? RePassword { get; set; } = string.Empty;
    }

}