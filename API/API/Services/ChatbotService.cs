using API.Common;
using API.Configurations;
using API.Helper;
using API.Models;
using API.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Text;

namespace API.Services
{
    public interface IChatbotService
    {
        public Task<Conversation> CreateConversation(string userId, string title);
        public Task<(string, List<Conversation>?)> GetUserConversations(string userId);
        public Task<List<Message>> GetMessagesByConversation(string conversationId);
        public Task<(string, Message?)> SendMessage(string userId, string conversationId, int role, string content);
        public Task<string> UpdateUserDailyUsage(string userId);

    }
    public class ChatbotService : IChatbotService
    {
        private readonly Exe201Context _context;
        private readonly HttpClient _httpClient;

        #region
        private readonly string AIApiKey = ConfigManager.gI().AiKey;
        private readonly string AIUri = "https://api.openai.com/v1/";

        #endregion

        public ChatbotService(Exe201Context context)
        {
            _context = context;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(AIUri)
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AIApiKey);
        }

        public async Task<Conversation> CreateConversation(string userId, string title)
        {
            var conversation = new Conversation
            {
                ConversationId = Guid.NewGuid().ToString(),
                UserId = userId,
                Title = title,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                ModelUsed = ConstMessage.CHATAI_DEFAULT_MODEL
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();
            return conversation;
        }

        public async Task<(string, List<Conversation>?)> GetUserConversations(string userId)
        {
            if (userId.IsEmpty()) return ("UserId is null!", null);

            var list = await _context.Conversations.Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.LastMessageAt)
                    .ToListAsync();

            return ("", list);
        }

        public async Task<List<Message>> GetMessagesByConversation(string conversationId)
        {
            return await _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }
        public async Task<(string, Message?)> SendMessage(string userId, string conversationId, int role, string content)
        {
            if (!await CanUserAskQuestion(userId))
                return ("Bạn đã đạt giới hạn câu hỏi trong ngày. Hãy nạp thêm hoặc đợi ngày mai.", null);

            await using var transaction = await _context.Database.BeginTransactionAsync(); // Mở transaction
            try
            {
                var conversation = await _context.Conversations
                    .FirstOrDefaultAsync(c => c.ConversationId == conversationId);
                if (conversation == null)
                    throw new Exception("Conversation not found");

                var userMessage = new Message
                {
                    MessageId = Guid.NewGuid().ToString(),
                    ConversationId = conversationId,
                    Role = role, // 0 = user, 1 = assistant
                    Content = content,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Messages.Add(userMessage);
                conversation.LastMessageAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(); 

                // Gửi toàn bộ lịch sử hội thoại lên AI
                var chatHistory = await _context.Messages
                    .Where(m => m.ConversationId == conversationId)
                    .OrderBy(m => m.CreatedAt)
                    .Select(m => new
                    {
                        role = m.Role == 0 ? "user" : "assistant",
                        content = m.Content
                    }).ToListAsync();

                var requestBody = new
                {
                    model = ConstMessage.CHATAI_DEFAULT_MODEL,
                    messages = chatHistory,
                    max_tokens = 200
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("chat/completions", jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                // Kiểm tra response có hợp lệ không
                dynamic responseData = JsonConvert.DeserializeObject(responseString);
                if (responseData?.choices == null || responseData.choices.Count == 0)
                    return ("Lỗi khi gọi API AI: Không nhận được phản hồi hợp lệ.", null);

                string aiResponse = responseData.choices[0].message.content;

                // Lưu tin nhắn AI vào DB
                var aiMessage = new Message
                {
                    MessageId = Guid.NewGuid().ToString(),
                    ConversationId = conversationId,
                    Role = (int)ChatbotRole.Assistant,
                    Content = aiResponse,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Messages.Add(aiMessage);
                conversation.LastMessageAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Cập nhật số lượng câu hỏi của user trong ngày
                await UpdateUserDailyUsage(userId);

                await transaction.CommitAsync();  

                return (string.Empty, aiMessage);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();  
                return ($"Lỗi khi gửi tin nhắn: {ex.Message}", null);
            }
        }

        public async Task<bool> CanUserAskQuestion(string userId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

            var usage = await _context.UserDailyUsages
                .FirstOrDefaultAsync(u => u.UserId == userId && u.UsageDate == today);
            
            // Nếu chưa có bản ghi, cho phép hỏi
            if (usage == null) return true;

            return usage.QuestionCount < usage.MaxQuestionsPerDay;
        }

        public async Task<string> UpdateUserDailyUsage(string userId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

            var usage = await _context.UserDailyUsages
                .FirstOrDefaultAsync(u => u.UserId == userId && u.UsageDate == today);

            if (usage == null)
            {
                usage = new UserDailyUsage
                {
                    UsageId = Guid.NewGuid().ToString(),
                    UserId = userId,
                    UsageDate = today,
                    QuestionCount = 1,
                    MaxQuestionsPerDay = 10,
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserDailyUsages.Add(usage);
            }
            else
            {
                usage.QuestionCount++;
                usage.UpdatedAt = DateTime.UtcNow;
                _context.UserDailyUsages.Update(usage);
            }

            await _context.SaveChangesAsync();
            return "";
        }
    }
}
