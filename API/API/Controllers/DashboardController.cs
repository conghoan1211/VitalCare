using API.Models;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _iService;
        public DashboardController(IDashboardService iService)
        {
            _iService = iService;
        }

        [HttpGet("get-revenue-date")]
        public async Task<IActionResult> GetRevenueDate([FromQuery] string period)
        {
            var (message, list) = await _iService.GetRevenueByDate(period);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Lấy doanh thu thành công.", data = list });
        }

        [HttpGet("get-user-statistics")]
        public async Task<IActionResult> GetUserStatistics()
        {
            var (message, result) = await _iService.GetUserStatistics();
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Lấy thống kê theo người dùng thành công.", data = result });
        }

        [HttpGet("get-category-statistics")]
        public async Task<IActionResult> GetCategoryStatistics()
        {
            var (message, result) = await _iService.GetCategoryStatistics();
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Lấy thống kê thể loại hàng thành công.", data = result });
        }
    }
}

