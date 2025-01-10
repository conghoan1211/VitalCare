using API.Services;
using API.ViewModels;
using API.ViewModels.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
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


        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateProfile(string userID, UpdateProfileModels? updatedProfile)
        {
            string msg = await _iProfileService.UpdateProfile(userID, updatedProfile, HttpContext);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok("Update Profile Successfully!");
        }

        [HttpPost("ChangeAvatar")]
        public async Task<IActionResult> DoChangeAvatar(string userID, UpdateAvatarVM input)
        {
            string msg = await _iProfileService.DoChangeAvatar(userID, input, HttpContext);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok("Update Avatar Successfully!");
        }
    }
}
