﻿using API.Services;
using API.ViewModels;
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

        [HttpGet("GetDetail")]
        public async Task<IActionResult> GetProductDetail(string productId)
        {
            var (msg, product) = await _iProductService.GetDetail(productId);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok(product);
        }

        [HttpGet("GetList")]
        public async Task<IActionResult> GetProductList(string productId)
        {
            var (msg, product) = await _iProductService.GetList();
            if (msg.Length > 0) return BadRequest(msg);
            return Ok(product);
        }

        [HttpPost("InsertUpdate")]
        public async Task<IActionResult> DoInsertUpdateProduct(string userId, InsertUpdateProductVM? input)
        {
            string msg = await _iProductService.DoInsertUpdate(input, userId);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok("Update Product Successfully!");
        }

        [HttpPost("ToggleActive")]
        public async Task<IActionResult> DoToggleActive(string userID, bool status)
        {
            string msg = await _iProductService.DoToggleActive(userID, status);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok("Update Product Successfully!");
        }
    }
}