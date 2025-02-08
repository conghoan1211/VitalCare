using API.Services;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

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

        [HttpGet("Detail")]
        public async Task<IActionResult> GetProductDetail(string productId)
        {
            var (message, product) = await _iProductService.GetDetail(productId);
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
                data = product
            });
        }

        [HttpGet("ListHome")]
        public async Task<IActionResult> GetProductList()
        {
            var (message, product) = await _iProductService.GetList();
            if (message.Length > 0)
            {
                return BadRequest(new  {
                    success = false, message,
                });
            }
            return Ok(new   {
                success = true, message = "Lấy danh sách thành công!", data = product
            });
        }

        [HttpGet("ListRelated")]
        public async Task<IActionResult> GetListRelated(string idExist)
        {
            var (message, product) = await _iProductService.GetListRelated(idExist);
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
                data = product
            });
        }

        [HttpGet("Search")]
        public async Task<IActionResult> DoSearch(string query)
        {
            var (message, product) = await _iProductService.DoSearch(query);
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
                data = product
            });
        }

        [HttpGet("FilterByCategoryId")]
        [EnableQuery]
        public async Task<IActionResult> GetProductList(int categoryId, string? sortBy)
        {
            var (message, product) = await _iProductService.FilterByCategoryId(categoryId, sortBy);
            if (message.Length > 0)
            {
                return BadRequest(new {
                    success = false,  message,
                });
            }
            return Ok(new  {
                success = true,  message = "Lấy danh sách thành công!", data = product
            });
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
