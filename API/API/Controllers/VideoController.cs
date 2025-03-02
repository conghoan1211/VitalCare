using API.Services;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : BaseController
    {
        private readonly IVideoService _iVideoService;
        public VideoController(IVideoService videoService)
        {
            _iVideoService = videoService;
        }

        [HttpGet("get-list")]
        public async Task<IActionResult> GetList()
        {
            var (message, list) = await _iVideoService.GetList(true, "created_dec");
            if (message.Length > 0)
            {
                return BadRequest(new  {
                    success = false,   message,
                });
            }
            return Ok(new   {
                success = true,  message = "Get list successfully!",  data = list
            });
        }

        [HttpGet("get-list-trend")]
        public async Task<IActionResult> GetListTrend()
        {
            var (message, list) = await _iVideoService.GetList(true, "view_dec");
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
                message = "Get list successfully!",
                data = list
            });
        }

        [HttpGet("get-list-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetListAdmin()
        {
            var (message, list) = await _iVideoService.GetList(false, "created_dec");
            if (message.Length > 0)
            {
                return BadRequest(new {
                    success = false, message,
                });
            }
            return Ok(new {
                success = true, message = "Get list successfully!", data = list
            });
        }

        [HttpGet("get-detail")]
        public async Task<IActionResult> GetDetail(string videoId)
        {
            var (message, detail) = await _iVideoService.GetDetail(videoId);
            if (message.Length > 0)
            {
                return BadRequest(new   {
                    success = false, message,
                });
            }
            return Ok(new  {
                success = true, message = "Get detail successfully!",  data = detail
            });
        }
        [HttpGet("list-cate")]
        public async Task<IActionResult> GetCategories()
        {
            var (message, detail) = await _iVideoService.GetCategory();
            if (message.Length > 0)
            {
                return BadRequest(new  {
                    success = false,  message,
                });
            }
            return Ok(new  {
                success = true, message = "Get detail successfully!",  data = detail
            });
        }

        [HttpPost("delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromBody] VideoRequest request)
        {
            var message = await _iVideoService.DeleteVideo(request.VideoId);
            if (message.Length > 0)
            {
                return BadRequest(new {
                    success = false,  message,
                });
            }
            return Ok(new {
                success = true,  message = "Delete successfully!",
            });
        }

        [HttpPut("update-views")]
        public async Task<IActionResult> UpdateViews( string videoId)
        {
            var message = await _iVideoService.UpdateViews(videoId);
            if (message.Length > 0)
            {
                return BadRequest(new  {
                    success = false,  message,
                });
            }
            return Ok(new  {
                success = true,  message = "Update views successfully!",
            });
        }

        [HttpPost("update-likes")]
        public async Task<IActionResult> UpdateLikes([FromQuery] string videoId)
        {
            var (message, likes) = await _iVideoService.UpdateLikes(videoId);
            if (message.Length > 0)
            {
                return BadRequest(new  {
                    success = false,  message, data= likes
                });
            }
            return Ok(new  {
                success = true,   message = "Update likes successfully!", data = likes
            });
        }

        [HttpPost("update-dislikes")]
        public async Task<IActionResult> UpdateDisLikes([FromQuery] string videoId)
        {
            var (message, likes) = await _iVideoService.UpdateDisLikes(videoId);
            if (message.Length > 0)
            {
                return BadRequest(new  {
                    success = false,  message, data = likes
                });
            }
            return Ok(new  {
                success = true,  message = "Update dislikes successfully!", data = likes
            });
        }

        [HttpPost("change-active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeActive([FromBody] VideoRequest request)
        {
            string userToken = GetUserId();
            var message = await _iVideoService.ChangeActive(request.VideoId, userToken);
            if (message.Length > 0)
            {
                return BadRequest(new  {
                    success = false,  message,
                });
            }
            return Ok(new  {
                success = true,  message = "Change active successfully!",
            });
        }

        [HttpPost("insert-update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> InsertUpdate([FromBody] InsertUpdateVideoVM input)
        {
            string userToken = GetUserId();
            string message = await _iVideoService.InsertUpdateVideo(input, userToken);
            if (message.Length > 0)
            {
                return BadRequest(new {
                    success = false,   message,
                });
            }
            return Ok(new   {
                success = true, message = "Insert or update successfully!",
            });
        }
    }
    public class VideoRequest
    {
        public string VideoId { get; set; }
    }
}
