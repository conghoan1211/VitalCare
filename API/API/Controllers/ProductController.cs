using API.Services;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _iProductService;
        public ProductController(IProductService iProductService)
        {
            _iProductService = iProductService;
        }
        #region User role

        [HttpGet("GetDetail")]
        public async Task<IActionResult> GetProductDetail(string productId)
        {
            var (msg, product) = await _iProductService.GetDetail(productId);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok(product);
        }

        [HttpGet("GetList")]
        public async Task<IActionResult> GetProductList()
        {
            var (msg, product) = await _iProductService.GetList();
            if (msg.Length > 0) return BadRequest(msg);
            return Ok(product);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> DoSearch(string query)
        {
            var (msg, product) = await _iProductService.DoSearch(query);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok(product);
        }

        [HttpGet("FilterByCategoryId")]
        public async Task<IActionResult> GetProductList(int categoryId)
        {
            var (msg, product) = await _iProductService.FilterByCategoryId(categoryId);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok(product);
        }
        #endregion

        #region Admin role
        [HttpPost("InsertUpdate")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DoInsertUpdateProduct(string userId, InsertUpdateProductVM? input)
        {
            string msg = await _iProductService.DoInsertUpdate(input, userId);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok("Update Product Successfully!");
        }

        [HttpPost("ToggleActive")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DoToggleActive(string userID, bool status)
        {
            string msg = await _iProductService.DoToggleActive(userID, status);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok("Update Product Successfully!");
        }

        [HttpPut("DeleteSoft")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DoToggleActive(string productId, string userId)
        {
            string msg = await _iProductService.DoDeleteSoft(productId, userId);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok("Update Product Successfully!");
        }
        #endregion
    }
}
