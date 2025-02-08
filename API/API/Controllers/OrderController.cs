using API.Services;
using API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _iOrderService;
        public OrderController(IOrderService iOrderService )
        {
            _iOrderService = iOrderService;
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] InsertOrderVM input)
        {
            var result = await _iOrderService.CreateOrder(input);
            if (result.Length > 0)
            {
                return BadRequest(new { success = false, message = result, errorCode = "CREATE_ORDER_FAILED" });
            }
            return Ok(new { success = true, message = "Tạo đơn hàng thành công." });
        }

        [HttpGet("GetAllOrder")]
        public async Task<IActionResult> GetAllOrder(int status = 0)
        {
            var (message, orders) = await _iOrderService.GetAll(status);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message, errorCode = "CREATE_ORDER_FAILED" });
            }
            return Ok(new { success = true, message = "Tạo đơn hàng thành công.", data= orders });
        }


        [HttpPost("SetStatus")]
        public async Task<IActionResult> SetStatus(string orderId, int status = 0)
        {
            var message = await _iOrderService.SetStatus(orderId, status);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message, errorCode = "CREATE_ORDER_FAILED" });
            }
            return Ok(new { success = true, message = "Tạo đơn hàng thành công." });
        }

        [HttpGet("GetListByUserId")]
        public async Task<IActionResult> GetOrderByUserId(string userId )
        {
            var (message, orders) = await _iOrderService.GetOrderByUserId(userId);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message, errorCode = "CREATE_ORDER_FAILED" });
            }
            return Ok(new { success = true, message = "Tạo đơn hàng thành công.", data = orders });
        }
    }
}
