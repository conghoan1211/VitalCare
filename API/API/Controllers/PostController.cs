using API.Common;
using API.Services;
using API.ViewModels;
using InstagramClone.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : BaseController
    {
        private readonly IPostService _iService; 
        private readonly JwtAuthentication _jwtAuthentic;

        public PostController(IPostService iService, JwtAuthentication jwtAuthentic)
        {
            _iService = iService;
            _jwtAuthentic = jwtAuthentic;
        }

        /// <summary>
        /// privacy = 0: public, 1: private
        /// 
        /// </summary>
        /// <param name="privacy"></param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<IActionResult> GetList(int privacy)
        {
            var (message, list) = await _iService.GetList(privacy);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message});
            }
            return Ok(new { success = true, message = "Lấy danh sách bài viết thành công.", data = list });
        }

        [HttpGet("GetListPopular")]
        public async Task<IActionResult> GetListPopular()
        {
            var (message, list) = await _iService.GetListPopular();
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Lấy danh sách bài viết phổ biến thành công.", data = list });
        }

        [HttpGet("GetDetail")]
        public async Task<IActionResult> GetDetail(string postId)
        {
            var (message, detail) = await _iService.GetDetail(postId);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Lấy bài viết thành công.", data = detail });
        }

        [HttpPost("InsertUpdate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> InsertUpdatePost([FromForm] InsertUpdatePost input)
        {
            var userId = GetUserId();
            string message = await _iService.InsertUpdatePost(input, userId.ToString());
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Tạo bài viết thành công."});
        }

        [HttpPut("DoChangePrivacy")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DoChangePrivacy([FromBody] ChangePrivacyRequest request)
        {
            string message = await _iService.DoChangePrivacy(request.PostId, request.Privacy);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Change Privacy Successfully." });
        }

        [HttpPost("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DoDeletePost([FromBody] string postId)
        {
            string message = await _iService.DoDeletePost(postId);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Delete Post Successfully." });
        }

        [HttpPut("update-views")]
        public async Task<IActionResult> UpdateViews(string postId)
        {
            string message = await _iService.UpdateViews(postId);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Update Views Successfully." });
        }
    }
    public class ChangePrivacyRequest
    {
        public string PostId { get; set; }
        public int Privacy { get; set; }
    }
}
