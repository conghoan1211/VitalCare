using API.Common;
using API.Configurations;
using API.Models;
using API.Services;
using API.Utilities;
using API.ViewModels;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2.Requests;
using InstagramClone.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenController : Controller
    {
        private readonly IAuthenticateService _iAuthenticate;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountService _iAccService;
        private readonly JwtAuthentication _jwtAuthen;
        private readonly Exe201Context _context;
        private readonly HttpClient _httpClient;

        public AuthenController(IAuthenticateService iAuthenticate, IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory, JwtAuthentication jwtAuthen, IAccountService iAccService, Exe201Context context)
        {
            _iAuthenticate = iAuthenticate;
            _httpContextAccessor = httpContextAccessor;
            _iAccService = iAccService;
            _jwtAuthen = jwtAuthen;
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
        }


        [HttpPost("google-callback1")]
        public async Task<IActionResult> GoogleCallback1([FromBody] GoogleAuthRequest request)
        {
            try
            {
                // Verify ID token
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { ConfigManager.gI().GoogleClientIp }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.Credential, settings);

                var userInfo = new GoogleUserInfo
                {
                    Email = payload.Email,
                    Name = payload.Name,
                    Picture = payload.Picture,
                    Id = payload.Subject,
                    VerifiedEmail = payload.EmailVerified,
                };

                var (msg, data) = await _iAuthenticate.LoginByGoogle(userInfo, _httpContextAccessor.HttpContext);
                if (!string.IsNullOrEmpty(msg))
                    return BadRequest(new { success = false, message = msg });


                return Ok(new
                {
                    success = true,
                    message = "Login successful",
                    data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, ex.Message });
            }
        }

        [HttpPost("google-callback")]
        public async Task<IActionResult> GoogleCallback([FromBody] GoogleAuthRequest request)
        {
            try
            {
                var tokenResponse = await GoogleAuthentication.GetAuthAccessTokenAsync(request.Credential, _httpContextAccessor.HttpContext);
                var userInfo = await GoogleAuthentication.GetUserInfoAsync(tokenResponse.AccessToken);
                var (msg, data) = await _iAuthenticate.LoginByGoogle(userInfo, _httpContextAccessor.HttpContext);
                if (msg.Length > 0) { return BadRequest(msg); }

                return Ok(new
                {
                    Message = msg,
                    Data = data,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message, ex.StackTrace });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLogin input)
        {
            var (msg, data) = await _iAuthenticate.DoLogin(input, HttpContext);
            if (msg.Length > 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = msg,
                    errorCode = "LOGIN_FAILED"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Đăng nhập thành công!",
                data
            });
        }

        [HttpPost("facebook-login")]
        public async Task<IActionResult> FacebookLogin([FromBody] FacebookLoginRequest request)
        {
            var verifyTokenUrl = $"https://graph.facebook.com/me?access_token={request.AccessToken}&fields=id,name,email";

            var response = await _httpClient.GetAsync(verifyTokenUrl);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new { message = "Invalid Facebook token." });
            }

            var userData = await response.Content.ReadAsStringAsync();
            var facebookUser = JsonConvert.DeserializeObject<FacebookUser>(userData);

            // Tùy chỉnh thêm: lưu thông tin người dùng vào database, tạo JWT token, v.v.
            return Ok(new
            {
                facebookUser.Id,
                facebookUser.Name,
                facebookUser.Email
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegister input)
        {
            var msg = await _iAuthenticate.DoRegister(input);
            if (msg.Length > 0)
            {
                return BadRequest( new {
                    success = false,  message = msg,
                });
            }

            return Ok( new {
                success = true, message = "Đăng ký thành công!",
            });
        }

        [HttpGet("validate-token")]
        public IActionResult ValidateToken(string refreshToken)
        {
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "No token found." });

            var data = _jwtAuthen.ParseJwtToken(token);
            if (data == null) // Token đã hết hạn
            {
                if (!string.IsNullOrEmpty(refreshToken))  
                {
                    var userRefresh = _context.Users.FirstOrDefault(x => x.RefreshToken == refreshToken);
                    if (userRefresh == null || userRefresh.ExpiryDateToken < DateTime.Now)
                    {
                        return Unauthorized(new { message = "Refresh token has expired" });
                    }
                    _jwtAuthen.GenerateJwtToken(userRefresh, HttpContext);
                    var newRefreshToken = _jwtAuthen.GenerateRefreshToken(userRefresh, _context, HttpContext);

                    return Ok(new
                    {
                        refreshToken = newRefreshToken,
                        isAuthenticated = true,
                    });
                }
                return Unauthorized(new { message = "Token không hợp lệ hoặc đã hết hạn." });
            }

            return Ok(new
            {
                isAuthenticated = true,  data 
            });
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new {message= "Refresh token is missing" });

            try
            {
                var userRefresh = await _context.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
                if (userRefresh == null || userRefresh.ExpiryDateToken < DateTime.Now)
                {
                    return Unauthorized(new { message = "Refresh token has expried" });
                }

                _jwtAuthen.GenerateJwtToken(userRefresh, HttpContext);  // lưu token vào cookie
                var newRefreshToken = _jwtAuthen.GenerateRefreshToken(userRefresh, _context, HttpContext);

                return Ok(new
                {
                    refreshToken = newRefreshToken.Result
                });
            }
            catch (Exception ex)
            {
                return Unauthorized($"Error: {ex.Message}");
            }
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword input)
        {
            if (input == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Input cannot be null"
                });
            }

            var msg = await _iAuthenticate.DoChangePassword(input);
            if (msg.Length > 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = msg,
                });
            }

            return Ok(new
            {
                success = true,
                message = "Thay đổi mật khẩu thành công!"
            });
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPassword input)
        {
            var msg = await _iAuthenticate.DoForgetPassword(input, HttpContext);
            if (msg.Length > 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = msg,
                    errorCode = "Reset_Failed"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Mật khẩu mới đã được gửi qua email của bạn!"
            });
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassword data)
        {
            var msg = await _iAuthenticate.DoResetPassword(data);
            if (msg.Length > 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = msg,
                    errorCode = "Reset_Failed"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Mật khẩu mới đã được thiết lập!"
            });
        }
        [HttpGet("logout")]
        public async Task<IActionResult> DoLogout(string userId)
        {
            var msg = await _iAuthenticate.DoLogout(HttpContext, userId);
            if (msg.Length > 0) return BadRequest(new { success = false, message = msg });
            return Ok(new { success = true, message = "Đăng xuất thành công!" });
        }

        // Request model
        public class FacebookLoginRequest
        {
            public string AccessToken { get; set; }
        }

        // Response model từ Facebook
        public class FacebookUser
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
        }

    }
}
