using API.Models;
using API.RabbitMQ;
using API.Services;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _iOrderService;
        public OrderController(IOrderService iOrderService)
        {
            _iOrderService = iOrderService;
        }

        [HttpPost("CreateOrder")]
        [Authorize]
        public IActionResult CreateOrder([FromBody] InsertOrderVM input)
        {
            var rabbit = new OrderProducer();
            rabbit.SendOrder(input);
            return Ok(new { success = true, message = "Đơn hàng đã được gửi về hệ thống để xử lý.", data = input });
        }

        [HttpGet("GetAllOrder")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrder()
        {
            var (message, orders) = await _iOrderService.GetAll();
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message, errorCode = "CREATE_ORDER_FAILED" });
            }
            return Ok(new { success = true, message = "Lấy danh sách đơn hàng thành công.", data = orders });
        }

        [HttpPost("SetStatus")]
        [Authorize]
        public async Task<IActionResult> SetStatus([FromBody] OrderStatusUpdateRequest? request)
        {
            var message = await _iOrderService.SetStatus(request.OrderId, request.Status);
            if (!string.IsNullOrEmpty(message))
            {
                return BadRequest(new { success = false, message, errorCode = "CREATE_ORDER_FAILED" });
            }

            return Ok(new { success = true, message = "Cập nhật trạng thái đơn hàng thành công." });
        }

        [HttpGet("GetListByUserId")]
        [Authorize]
        public async Task<IActionResult> GetOrderByUserId(string userId)
        {
            var (message, orders) = await _iOrderService.GetOrderByUserId(userId);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message, data = orders });
            }
            return Ok(new { success = true, message = "Lấy đơn hàng thành công.", data = orders });
        }
    }

    public class OrderStatusUpdateRequest
    {
        [Required(ErrorMessage = "OrderId is required")]
        public string? OrderId { get; set; }
        [Required(ErrorMessage = "Status is required")]
        public int Status { get; set; }
    }
}
