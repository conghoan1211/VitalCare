using API.Common;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : BaseController
    {
        private readonly IChatbotService _chatbotService;
        public ChatbotController(IChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
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
            if (messages == null || messages.Count <= 0)
            {
                return BadRequest(new { success = false, message = "Chưa có tin nhắn nào." });
            }
            return Ok(new { success = true, message = "Lấy list tin nhắn thành công.", data = messages });
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var userId = GetUserId();
            if (userId.IsEmpty()) return BadRequest("User is not valid!");

            var (message, newMessage)  = await _chatbotService.SendMessage(userId, request.ConversationId, request.Role, request.Content);
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
}
