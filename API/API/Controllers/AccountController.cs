using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _iService;
        public AccountController(IAccountService iService)
        {
            _iService = iService;
        }

        [HttpGet("GetList")]
        public async Task<IActionResult> GetCategoryList()
        {
            var (msg, list) = await _iService.GetList();
            if (msg.Length > 0) return BadRequest(msg);
            return Ok(list);
        }

        [HttpPost("ToggleActive")]
        public async Task<IActionResult> DoToggleActive(string userId, string usertoken, bool active)
        {
            string msg = await _iService.DoToggleActive(userId,usertoken,active);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok("Update Category Successfully!");
        }

        [HttpGet("DoSearch")]
        public async Task<IActionResult> DoSearch(string query)
        {
            var (msg, list) = await _iService.DoSearch(query);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok(list);
        }
    }
}
