using API.Models;
using API.Services;
using API.ViewModels;
using API.ViewModels.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProfileService _iProfileService;
        public ProfileController(IProfileService iProfileService, IHttpContextAccessor httpContextAccessor)
        {
            _iProfileService = iProfileService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Lấy thông tin người dùng bằng ID 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("GetUserByID")]
        public async Task<IActionResult> GetUserByID(string userId)
        {
            var (msg, user) = await _iProfileService.GetProfile(userId);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok(user);
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetProfile(string userId)
        {
            var (message, user) = await _iProfileService.GetProfileUpdate(userId);
            if (message.Length > 0)
            {
                return BadRequest( new {
                    success = false,  message,  errorCode = "GET_FAILED"
                });
            }
            return Ok( new {
                success = true,  message = "Đã lấy được dữ liệu.",   data = user
            });
        }


        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModels updatedProfile)
        {
            if (updatedProfile == null)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errorCode = "INVALID_REQUEST" });
            }
            string message = await _iProfileService.UpdateProfile(updatedProfile, HttpContext);
            if (message.Length > 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message,
                });
            }
            return Ok(new
            {
                success = true,
                message = "Update Profile Successfully!",
            });
        }

        [HttpPost("ChangeAvatar")]
        public async Task<IActionResult> DoChangeAvatar(string userID, UpdateAvatarVM input)
        {
            if (input.Image == null || input.Image.Length == 0)
            {
                return BadRequest(new  {
                    success = false,  message = "Không có tệp avatar.", errorCode = "NO_FILE"
                });
            }
             var (message, url) = await _iProfileService.DoChangeAvatar(userID, input, HttpContext);
            if (message.Length > 0)
            {
                return BadRequest( new {
                    success = false,  message,  errorCode = "CHANGE_FAILED"
                });
            }
            return Ok( new {
                success = true,  message = "Đổi avatar thành công.", data = url
            });
        }
    }
}
