using API.Models;
using API.Services;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _iCategoryService;
        public CategoryController(ICategoryService iCategoryService)
        {
            _iCategoryService = iCategoryService;
        }

        [HttpGet("List")]
        public async Task<IActionResult> GetCategoryList(bool? isActive, int? typeCateria = null)
        {
            var (message, list) = await _iCategoryService.GetList(isActive, typeCateria);
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
                message = "Lấy danh sách thành công!",
                data = list
            });
        }

        [HttpGet("GetDetail")]
        public async Task<IActionResult> GetCategoryDetail(int categoryId, bool? isActive = null, int? typeCateria = null)
        {
            var (msg, list) = await _iCategoryService.GetDetail(categoryId, isActive, typeCateria);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok(list);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("InsertUpdate")]
        public async Task<IActionResult> DoInsertUpdateProduct(InsertUpdateCategory? input)
        {
            string msg = await _iCategoryService.DoInsertUpdate(input);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok("Update Category Successfully!");
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("ToggleActive")]
        public async Task<IActionResult> DoToggleActive(int? categoryId, bool active)
        {
            string msg = await _iCategoryService.DoToggleActive(categoryId, active);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok("Update Category Successfully!");
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("DeleteSoft")]
        public async Task<IActionResult> DoToggleActive(int? categoryId )
        {
            string msg = await _iCategoryService.DoDeleteSoft(categoryId );
            if (msg.Length > 0) return BadRequest(msg);
            return Ok("Update Category Successfully!");
        }
    }
}
