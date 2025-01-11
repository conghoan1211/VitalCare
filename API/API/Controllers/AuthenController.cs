using API.Configurations;
using API.Services;
using API.Utilities;
using API.ViewModels;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenController : Controller
    {
        private readonly IAuthenticateService _iAuthenticate;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;

        public AuthenController(IAuthenticateService iAuthenticate, IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory)
        {
            _iAuthenticate = iAuthenticate;
            _httpContextAccessor = httpContextAccessor;
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
                    return BadRequest(new { Message = msg });

                return Ok(new
                {
                    Message = "Login successful",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
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
                return StatusCode(500, new {ex.Message, ex.StackTrace});
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLogin input)
        {
            var (msg, data) = await _iAuthenticate.DoLogin(input, _httpContextAccessor.HttpContext);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok(data);
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


        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword input)
        {
            var msg = await _iAuthenticate.DoChangePassword(input);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok(input.Password);
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPassword input)
        {
            var msg = await _iAuthenticate.DoForgetPassword(input, HttpContext);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok(input);
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
