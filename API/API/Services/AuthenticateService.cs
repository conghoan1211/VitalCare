﻿using API.Common;
using API.Helper;
using API.Models;
using API.Utilities;
using API.ViewModels;
using AutoMapper;
using Google.Apis.Auth;
using InstagramClone.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Security.Cryptography;
using zaloclone_test.Helper;

namespace API.Services
{
    public interface IAuthenticateService
    {
        public Task<(string, LoginGoogleResult)> LoginByGoogle(GoogleUserInfo input, HttpContext httpContext);

        public Task<(string, LoginResult?)> DoLogin(UserLogin userLogin, HttpContext httpContext);
        public Task<string> DoRegister(UserRegister userRegister);
        public Task<string> DoLogout(HttpContext httpContext, string phone);
        public Task<string> DoForgetPassword(ForgetPassword input, HttpContext httpContext);
        public Task<string> DoVerifyOTP(string otp, HttpContext httpContext);
        public Task<string> DoResetPassword(ResetPassword input);
        public Task<string> DoChangePassword(ChangePassword input);
        public Task<(string message, User? user)> DoSearchByEmail(string? email);
        public Task<(string message, User? user)> DoSearchByPhone(string? phone);
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
            user.UpdateAt = DateTime.Now;
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
                (msg, newpass) = await EmailHandler.SendPasswordAndSaveSession(input.Email, httpContext);
                if (msg.Length > 0) return msg;

                httpContext.Session.Remove("newPassword");
                msg = Converter.StringToMD5(newpass, out string mkMd5);
                if (msg.Length > 0) return $"Error convert to MD5: {msg}";

                user.Password = mkMd5;
                user.UpdateUser = user.UserId;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            return "";
        }

        public async Task<(string, LoginResult?)> DoLogin(UserLogin userLogin, HttpContext httpContext)
        {
            string msg = "";
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == userLogin.Username || x.Email == userLogin.Username);
            if (user is null) return ("Tài khoản không tồn tại.", null);

            msg = Converter.StringToMD5(userLogin.Password, out string mkMd5);
            if (msg.Length > 0) return ($"Error convert to MD5: {msg}", null);
            if (!user.Password.ToUpper().Equals(mkMd5.ToUpper())) return ("Mật khẩu không chính xác", null);

            if (user.IsVerified == false) return (ConstMessage.ACCOUNT_UNVERIFIED, null);
            if (user.IsActive == false) return ($"Tài khoản của bạn đã bị khóa đến ngày {user.BlockUntil}", null);

            user.Status = (int)UserStatus.Active;
            await _context.SaveChangesAsync();

            var token = _jwtAuthen.GenerateJwtToken(user, httpContext);
            var userDto = _mapper.Map<UserVM>(user);

            return ("", new LoginResult
            {
                Token = token,
                User = userDto
            });
        }


        public async Task<(string, LoginGoogleResult)> LoginByGoogle(GoogleUserInfo input, HttpContext httpContext)
        {
            bool hasPassword = true;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == input.Email);
            if (user == null)
            {
                var userid = Guid.NewGuid().ToString();

                user = new User
                {
                    UserId = userid,
                    Username = input.Nickname ?? $"user_{userid.Substring(0, 8)}",
                    Phone = input.PhoneNumber,
                    Email = input.Email,
                    Password = null,
                    RoleId = (int)Role.User,
                    Status = (int)UserStatus.Inactive,
                    IsActive = true,
                    IsDisable = false,
                    IsVerified = true,
                    CreateAt = DateTime.Now,
                    CreateUser = userid,
                    Bio= input.ProfileLink,
                    Avatar = "default-avatar.png",
                };
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                hasPassword = false;
            }
            hasPassword = !string.IsNullOrEmpty(user.Password);
            if (user.IsActive == false)
            {
                return ("Tài khoản đã bị vô hiệu hóa.", null);
            }

            var token = _jwtAuthen.GenerateJwtToken(user, httpContext);
            var userDto = _mapper.Map<UserVM>(user);

            return ("", new LoginGoogleResult
            {
                HasPassword = hasPassword,
                Token = token,
                User = userDto
            });
        }

        public async Task<string> DoLogout(HttpContext httpContext, string? phone)
        {
            var (msg, user) = await DoSearchByPhone(phone);
            if (msg.Length > 0) return msg;
            else if (user != null)
            {
                user.Status = (int)UserStatus.Inactive;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            httpContext.Response.Cookies.Delete("JwtToken");
            httpContext.Session.Clear();
            return "";
        }

        public async Task<string> DoRegister(UserRegister input)
        {
            string msg = "";
            msg = _context.Users.CheckPhone(input.Phone);
            if (msg.Length > 0) return msg;

            msg = _context.Users.CheckEmail(input.Email);
            if (msg.Length > 0) return msg;

            msg = Converter.StringToMD5(input.Password, out string mkMd5);
            if (msg.Length > 0) return $"Error convert to MD5: {msg}";

            var userid = Guid.NewGuid().ToString();
            var user = new User
            {
                UserId = userid,
                Username = input.UserName,
                Phone = input.Phone,
                Email = input.Email,
                Password = mkMd5,
                RoleId = (int)Role.User,
                Sex = input.Sex,
                Dob = input.Dob,
                Status = (int)UserStatus.Inactive,
                IsActive = true,
                IsDisable = false,
                IsVerified = false,
                CreateAt = DateTime.Now,
                UpdateAt = null,
                CreateUser = userid,
                Avatar = "default-avatar.png",
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return "";
        }

        public Task<string> DoResetPassword(ResetPassword input)
        {
            throw new NotImplementedException();
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

    }
}