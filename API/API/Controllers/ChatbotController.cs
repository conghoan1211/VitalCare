using API.Common;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatbotController : BaseController
    {
        private readonly IChatbotService _chatbotService;
        public ChatbotController(IChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        [HttpGet("create-conversation")]
        public async Task<IActionResult> CreateConversation()
        {
            var userId = GetUserId();
            if (userId.IsEmpty())
            return BadRequest(new { success = false, message = "User is not valid." });

            var conversation = await _chatbotService.CreateConversation(userId);
            if (conversation == null)
            {
                return BadRequest(new { success = false, message = "Tạo hội thoại thất bại." });
            }
            return Ok(new { success = true, message = "Tạo hội thoại thành công.", data = conversation });
        }

        [HttpGet("get-conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var userId = GetUserId();
            if (userId.IsEmpty()) return BadRequest("User is not valid!");

            var (message, conversations) = await _chatbotService.GetUserConversations(userId);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Lấy hội thoại thành công.", data = conversations });
        }

        [HttpGet("get-messages")]
        public async Task<IActionResult> GetMessages(string conversationId)
        {
            var userId = GetUserId();
            if (userId.IsEmpty()) return BadRequest("User is not valid!");

            var messages = await _chatbotService.GetMessagesByConversation(conversationId);
            return Ok(new { success = true, message = "Lấy list tin nhắn thành công.", data = messages });
        }

        [HttpPut("update-title-conversation")]
        public async Task<IActionResult> UpdateTitle([FromBody] RenameConversationRequest request)
        {
            var message = await _chatbotService.RenameConversation(request.ConversationId, request.Title);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Cập nhật tiêu đề hội thoại thành công.", data = request.Title });
        }

        [HttpPost("delete-conversation")]
        public async Task<IActionResult> DeleteConversation([FromBody] string conversationId)
        {
            var message = await _chatbotService.DeleteConversation(conversationId);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Xóa hội thoại thành công." });
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest requests)
        {
            var userId = GetUserId();
            if (userId.IsEmpty()) return BadRequest("User is not valid!");

            var (message, newMessage)  = await _chatbotService.SendMessage(userId, requests.ConversationId, requests.Role, requests.Content);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Gửi tin nhắn thành công.", data = newMessage });
        }

    }
    public class SendMessageRequest
    {
        public string ConversationId { get; set; }
        public string Content { get; set; }
        public int Role{ get; set; }
    }
    // Định nghĩa class để nhận request
    public class RenameConversationRequest
    {
        public string ConversationId { get; set; }
        public string Title { get; set; }
    }
}
