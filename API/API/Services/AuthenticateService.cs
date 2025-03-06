using API.Common;
using API.Helper;
using API.Models;
using API.Utilities;
using API.ViewModels;
using API.ViewModels.Token;
using AutoMapper;
using Azure;
using InstagramClone.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace API.Services
{
    public interface IAuthenticateService
    {
        public Task<(string, LoginResult?)> LoginByGoogle(GoogleUserInfo input, HttpContext httpContext);
        public Task<(string, LoginResult?)> DoLogin(UserLogin userLogin, HttpContext httpContext);
        public Task<string> DoRegister(UserRegister userRegister);
        public Task<string> DoLogout(HttpContext httpContext, string userid);
        public Task<string> DoForgetPassword(ForgetPassword input, HttpContext httpContext);
        public Task<string> DoVerifyOTP(string otp, HttpContext httpContext);
        public Task<string> DoResetPassword(ResetPassword input);
        public Task<string> DoChangePassword(ChangePassword input);
        public Task<(string message, User? user)> DoSearchByEmail(string? email);
        public Task<(string message, User? user)> DoSearchByPhone(string? phone);
        //public Task<string> ValidateRefreshToken(User user, HttpContext httpContext);
    }

    public class AuthenticateService : IAuthenticateService
    {
        private readonly IMapper _mapper;
        private readonly Exe201Context _context;
        private readonly JwtAuthentication _jwtAuthen;

        public AuthenticateService(IMapper mapper, Exe201Context context, JwtAuthentication jwtAuthen)
        {
            _mapper = mapper;
            _context = context;
            _jwtAuthen = jwtAuthen;
        }

        public async Task<string> DoChangePassword(ChangePassword input)
        {
            var user = await _context.Users.FindAsync(input.UserId);
            if (user == null) return "Người dùng không tồn tại.";

            string msg = Converter.StringToMD5(input.ExPassword, out string? exPassMd5);
            if (msg.Length > 0) return $"Lỗi khi mã hóa mật khẩu cũ: {msg}";

            if (!user.Password.Equals(exPassMd5)) return "Mật khẩu hiện tại chưa chính xác.";
            if (input.Password.Equals(input.ExPassword)) return "Mật khẩu mới phải khác mật khẩu cũ.";

            msg = Converter.StringToMD5(input.Password, out string newPasswordMd5);
            if (msg.Length > 0) return $"Lỗi khi mã hóa mật khẩu mới: {msg}";

            user.Password = newPasswordMd5;
            user.UpdateUser = user.UserId;
            user.UpdateAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return "";
        }

        public async Task<string> DoForgetPassword(ForgetPassword input, HttpContext httpContext)
        {
            var (msg, user) = await DoSearchByEmail(input.Email);
            if (msg.Length > 0) return msg;
            else if (user != null)
            {
                string newpass = "";
                (msg, newpass) = await EmailHandler.SendEmailAndPassword(input.Email, httpContext);
                if (msg.Length > 0) return msg;

                msg = Converter.StringToMD5(newpass, out string mkMd5);
                if (msg.Length > 0) return $"Error convert to MD5: {msg}";

                user.Password = mkMd5;
                user.UpdateUser = user.UserId;
                user.UpdateAt = DateTime.UtcNow;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            return "";
        }

        public async Task<(string, LoginResult?)> DoLogin(UserLogin userLogin, HttpContext httpContext)
        {
            string msg = "";
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userLogin.Username);
            if (user is null) return ("Tài khoản không tồn tại.", null);

            msg = Converter.StringToMD5(userLogin.Password, out string mkMd5);
            if (msg.Length > 0) return ($"Error convert to MD5: {msg}", null);
            if (user.Password.IsEmpty()) return ("Tài khoản chưa có mật khẩu. Đăng nhập lại bằng Google.", null);
            if (!user.Password.ToUpper().Equals(mkMd5.ToUpper())) return ("Mật khẩu không chính xác", null);

            if (user.IsVerified == false) return (ConstMessage.ACCOUNT_UNVERIFIED, null);
            if (user.IsActive == false) return ($"Tài khoản đã bị vô hiệu hóa, liên hệ Admin để biết thêm chi tiết!", null);

            user.Status = (int)UserStatus.Active;
            user.LastLogin = DateTime.UtcNow;
            user.LastLoginIp = httpContext.Connection.RemoteIpAddress?.ToString();
            await _context.SaveChangesAsync();

            var accessToken = _jwtAuthen.GenerateJwtToken(user, httpContext);
            var refreshToken = await _jwtAuthen.GenerateRefreshToken(user, _context, httpContext);
            var userDto = _mapper.Map<UserLoginVM>(user);

            return ("", new LoginResult
            {
                RefreshToken = refreshToken,
                AccessToken = accessToken,
                HasPassword = true,
                Data = userDto
            });
        }

        public async Task<(string, LoginResult?)> LoginByGoogle(GoogleUserInfo input, HttpContext httpContext)
        {
            bool hasPassword = true;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.GoogleId == input.Id || x.Email == input.Email);
            if (user == null)
            {
                var userid = Guid.NewGuid().ToString();
                user = new User
                {
                    UserId = userid,
                    GoogleId = input.Id,
                    Username = input.Name ?? $"user_{userid.Substring(0, 8)}",
                    Phone = input.PhoneNumber,
                    Email = input.Email,
                    Password = null,
                    RoleId = (int)Role.User,
                    Status = (int)UserStatus.Inactive,
                    IsActive = true,
                    IsDisable = false,
                    IsVerified = input.VerifiedEmail,
                    CreateAt = DateTime.UtcNow,
                    CreateUser = userid,
                    LastLogin = DateTime.UtcNow,
                    LastLoginIp = httpContext.Connection.RemoteIpAddress?.ToString(),
                    Bio = input.ProfileLink,
                    Avatar = input.Picture ?? "default-avatar.png",
                };
                await _context.Users.AddAsync(user);
                hasPassword = false;
            }
            else                                              // improve this 
            {
                user.GoogleId = input.Id;
                user.LastLogin = DateTime.UtcNow;
                user.LastLoginIp = httpContext.Connection.RemoteIpAddress?.ToString();
                hasPassword = !string.IsNullOrEmpty(user.Password);
                _context.Users.Update(user);
            }
            await _context.SaveChangesAsync();

            if (user.IsActive == false) return ("Tài khoản đã bị vô hiệu hóa.", null);

            var accessToken =  _jwtAuthen.GenerateJwtToken(user, httpContext);
            var refreshToken = await _jwtAuthen.GenerateRefreshToken(user, _context, httpContext);
            var userDto = _mapper.Map<UserLoginVM>(user);

            return ("", new LoginResult
            {
                HasPassword = hasPassword,
                RefreshToken = refreshToken,
                AccessToken = accessToken,
                Data = userDto
            });
        }
        public async Task<string> DoLogout(HttpContext httpContext, string? userid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userid);
            if (user == null) return "User not found!";

            user.Status = (int)UserStatus.Inactive;
            user.LastLogin = DateTime.UtcNow;
            user.RefreshToken = null;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            httpContext.Response.Cookies.Delete("JwtToken");
            httpContext.Response.Cookies.Delete("RefreshToken");
            httpContext.Session.Clear();
            return "";
        }

        public async Task<string> DoRegister(UserRegister input)
        {
            string msg = "";
            //msg = _context.Users.CheckPhone(input.Phone);
            //if (msg.Length > 0) return msg;

            msg = _context.Users.CheckEmail(input.Email);
            if (msg.Length > 0) return msg;

            msg = Converter.StringToMD5(input.Password, out string mkMd5);
            if (msg.Length > 0) return $"Error convert to MD5: {msg}";

            var userid = Guid.NewGuid().ToString();
            var user = new User
            {
                UserId = userid,
                Username = input.UserName,
                Email = input.Email,
                Password = mkMd5,
                RoleId = (int)Role.User,
                Status = (int)UserStatus.Inactive,
                IsActive = true,
                IsDisable = false,
                IsVerified = false,
                CreateAt = DateTime.UtcNow,
                UpdateAt = null,
                CreateUser = userid,
                Avatar = "default-avatar.png",
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return "";
        }

        public async Task<string> DoResetPassword(ResetPassword input)
        {
            if (input.UserId == null) return "UserId is null";

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == input.UserId);
            if (user == null) return "Người dùng không tồn tại.";

            if (!user.Password.IsEmpty()) return "Tài khoản đã có mật khẩu.";

            var msg = Converter.StringToMD5(input.RePassword, out string mkMd5);
            if (msg.Length > 0) return $"Error convert to MD5: {msg}";

            user.Password = mkMd5;
            user.UpdateUser = user.UserId;
            user.UpdateAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return "";
        }

        public async Task<string> DoVerifyOTP(string otp, HttpContext httpContext)
        {
            var storedOtp = httpContext.Session.GetString("Otp");
            if (string.IsNullOrEmpty(storedOtp)) return "OTP đã hết hạn";
            if (otp != storedOtp) return "Mã OTP nhập không hợp lệ!";

            var emailVerify = httpContext.Session.GetString("email_verify");
            if (string.IsNullOrEmpty(emailVerify)) return "Vui lòng đăng nhập lại để được verify tài khoản.";

            var (msg, user) = await DoSearchByEmail(emailVerify);
            if (msg.Length > 0) return msg;
            else if (user != null)
            {
                user.IsVerified = true;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            httpContext.Session.Remove("Otp");
            httpContext.Session.Remove("email_verify");

            return string.Empty;
        }

        public async Task<(string message, User? user)> DoSearchByEmail(string? email)
        {
            if (string.IsNullOrEmpty(email) || !email.IsValidEmailFormat())
                return ("Email không hợp lệ", null);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null) return ("Tài khoản Email không tồn tại.", null);

            return (string.Empty, user);
        }

        public async Task<(string message, User? user)> DoSearchByPhone(string? phone)
        {
            if (string.IsNullOrEmpty(phone)) return ("Số điện thoại không hợp lệ", null);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Phone == phone);
            if (user == null) return ("Tài khoản không tồn tại.", null);

            return (string.Empty, user);
        }

        public async Task<UserToken?> ValidateRefreshToken(string refreshToken, string token)
        {
            var userRefresh = await _context.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);

            if (userRefresh == null || userRefresh.ExpiryDateToken < DateTime.UtcNow)
            {
                return null;
            }

            var userToken = _jwtAuthen.ParseJwtToken(token);
            return userToken;
        }
    }
}
