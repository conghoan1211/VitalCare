using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AccountController : BaseController
    {
        private readonly IAccountService _iService;
        public AccountController(IAccountService iService)
        {
            _iService = iService;
        }

        [HttpGet("GetList")]
        public async Task<IActionResult> GetAccountList()
        {
            var (message, list) = await _iService.GetList();
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Lấy danh sách tài khoản thành công.", data = list });
        }

        [HttpPost("ToggleActive")]
        public async Task<IActionResult> DoToggleActive( bool active, string userId)
        {
            var userToken = GetUserId();
            string message = await _iService.DoToggleActive(userToken, active, userId);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Thao tác thành công." });
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
