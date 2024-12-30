using API.Configurations;
using API.Services;
using API.Utilities;
using API.ViewModels;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenController : Controller
    {
        private readonly IAuthenticateService _iAuthenticate;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthenController(IAuthenticateService iAuthenticate, IHttpContextAccessor httpContextAccessor)
        {
            _iAuthenticate = iAuthenticate;
            _httpContextAccessor = httpContextAccessor;
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

        // POST: AuthenController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
