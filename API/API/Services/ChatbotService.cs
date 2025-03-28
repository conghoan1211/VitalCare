using API.Common;
using API.Configurations;
using API.Helper;
using API.Models;
using API.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace API.Services
{
    public interface IChatbotService
    {
        public Task<ConversationVm> CreateConversation(string userId);
        public Task<(string, List<ConversationVm>?)> GetUserConversations(string userId);
        public Task<List<MessageVm>> GetMessagesByConversation(string conversationId);
        public Task<(string, MessageVm?)> SendMessage(string userId, string conversationId, int role, string content);
        public Task<string> UpdateUserDailyUsage(string userId);
        public Task<string> DeleteConversation(string conversationId);
        public Task<string> RenameConversation(string conversationId, string title);

    }
    public class ChatbotService : IChatbotService
    {
        private readonly Exe201Context _context;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        #region
        private readonly string AIApiKey = ConfigManager.gI().AiKey;
        private readonly string AIUri = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

        private readonly string InitialSystemPrompt = @"Bạn là trợ lý AI của VitalCare, chuyên tư vấn về sức khỏe xương khớp. Chỉ trả lời các nội dung liên quan đến cơ, xương, khớp, các sản phẩm và nội dung hỗ trợ chăm sóc xương khớp. 
                                        Trả lời ngắn gọn, dễ hiểu, ưu tiên giải pháp tự nhiên, bài tập hỗ trợ và khuyên người dùng nên gặp bác sĩ khi cần thiết.
                                   Chú ý: Sau khi trả lời câu hỏi, hãy nói cho người dùng rằng: 'Câu trả lời trên chỉ mang tính chất tham khảo, quyết định cuối cùng vẫn phụ thuộc vào bạn, vui lòng tham khảo ý kiến bác sĩ hoặc chuyên gia trước khi quyết định'.";

        private readonly string SecondarySystemPrompt = @"Website cung cấp sản phẩm (sữa, miếng dán, thực phẩm dinh dưỡng) và nội dung xương khớp.\n
                                        Hướng người dùng vào danh sách sản phẩm nếu hỏi cụ thể, trả lời trọn vẹn, cô đọng, tối đa 600 token, ít dùng đậm/nghiêng.";

        private readonly string StrictScopePrompt = @"Chú ý: Tuyệt đối không trả lời các câu hỏi không liên quan đến cơ xương khớp. Nếu phát hiện nội dung hỏi không đúng phạm vi,
                                        hãy lịch sự từ chối và nhắc rằng bạn chỉ hỗ trợ chuyên sâu về sức khỏe cơ xương khớp. Không cố gắng đưa ra câu trả lời cho các lĩnh vực khác.";

        //private readonly string UseSystemPrompt = @"Đặt hàng: thêm vào giỏ, kiểm tra, nhập địa chỉ, chọn thanh toán, xác nhận. Cập nhật thông tin/đơn hàng: vào profile qua avatar góc trên phải";

        //private readonly string ImportantSystemPrompt = @"Khi người dùng hỏi ai đã sáng lập hay phát triển ra website VitalCare. Trả lời là do 1 nhóm sinh viên trường đại học FPT Hà Nội phát triển.
        //                                trong đó bên Marketing, nghiên cứu thị trường là các bạn: Lê Nguyễn Tùng Dương, Lương Tuệ Quang, Nguyễn Trà My. Bên phát triển Web là : Phạm Công Hoan, Cao Trường Sơn, Chu Thiên Quân. ";
        #endregion

        public ChatbotService(IMapper mapper, Exe201Context context, HttpClient httpClient)
        {
            _context = context;
            _mapper = mapper;
            _httpClient = httpClient;
        }

        private async Task<List<object>> GetFormattedChatHistory(string conversationId)
        {
            var chatHistory = new List<object>();


            // Kiểm tra xem hội thoại này đã có tin nhắn nào từ AI (model) chưa
            //bool hasAIResponse = await _context.Messages
            //    .AnyAsync(m => m.ConversationId == conversationId && m.Role == 1);

            // Nếu chưa có tin nhắn từ AI (tức là lần đầu mở), thêm 4 prompt hệ thống
            //if (!hasAIResponse)
            //{
            chatHistory.Add(new { role = "model", parts = new[] { new { text = InitialSystemPrompt } } });
            chatHistory.Add(new { role = "model", parts = new[] { new { text = SecondarySystemPrompt } } });
            chatHistory.Add(new { role = "model", parts = new[] { new { text = StrictScopePrompt } } });
            //}

            // Lấy danh sách tin nhắn của cuộc hội thoại
            var messages = await _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAt)
                .Select(m => new
                {
                    role = m.Role == 0 ? "user" : "model",
                    content = m.Content
                })
                .ToListAsync();

            var recentMessages = messages.TakeLast(5); // Chỉ lấy 5 tin gần nhất
            chatHistory.AddRange(recentMessages.Select(m => new
            {
                role = m.role,
                parts = new[] { new { text = m.content } }
            }));

            return chatHistory;
        }

        public async Task<(string, MessageVm?)> SendMessage(string userId, string conversationId, int role, string content)
        {
            if (!await CanUserAskQuestion(userId))
                return ("Bạn đã đạt giới hạn câu hỏi trong ngày. Hãy nạp thêm hoặc đợi ngày mai.", null);

            await using var transaction = await _context.Database.BeginTransactionAsync(); // Mở transaction
            try
            {
                var conversation = await _context.Conversations
                    .FirstOrDefaultAsync(c => c.ConversationId == conversationId);
                if (conversation == null)
                    return ("Conversation not found", null);

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
                if (conversation.Title.IsEmpty() || conversation.Title.Contains(ConstMessage.CONVERSATION_DEFAULT_TITLE))
                {
                    conversation.Title = content.Length > 30 ? content.Substring(0, 30) : content;
                }
                _context.Conversations.Update(conversation);
                await _context.SaveChangesAsync();

                // Gửi toàn bộ lịch sử hội thoại lên AI
                var chatHistory = await GetFormattedChatHistory(conversationId);

                var requestBody = new
                {
                    contents = chatHistory,
                    generationConfig = new
                    {
                        temperature = 0.85,
                        //topP = 0.7, // Giảm xuống
                        //topK = 100, // Tăng lên
                        maxOutputTokens = 600,
                        responseMimeType = "text/plain"
                    },
                    safetySettings = new[] {
                        new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_MEDIUM_AND_ABOVE" },
                        new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_MEDIUM_AND_ABOVE" }
                    }
                };

                var jsonPayload = JsonConvert.SerializeObject(requestBody, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented // Cho dễ đọc
                });
                var jsonContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{AIUri}?key={AIApiKey}", jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                // Kiểm tra response có hợp lệ không
                dynamic responseData = JsonConvert.DeserializeObject(responseString);
                if (responseData?.candidates == null || responseData.candidates.Count == 0)
                    return ("Lỗi khi gọi API AI: Không nhận được phản hồi hợp lệ.", null);

                string aiResponse = responseData.candidates[0].content.parts[0].text;
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

                var mapper = _mapper.Map<MessageVm>(aiMessage);
                return (string.Empty, mapper);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ($"Lỗi khi gửi tin nhắn: {ex.Message}", null);
            }
        }

        public async Task<ConversationVm> CreateConversation(string userId)
        {
            var conversation = new Conversation
            {
                ConversationId = Guid.NewGuid().ToString(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                ModelUsed = ConstMessage.CHATAI_DEFAULT_MODEL,
                Title = ConstMessage.CONVERSATION_DEFAULT_TITLE
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            var mapper = _mapper.Map<ConversationVm>(conversation);
            return mapper;
        }
        public async Task<(string, List<ConversationVm>?)> GetUserConversations(string userId)
        {
            if (userId.IsEmpty()) return ("UserId is null!", null);

            var list = await _context.Conversations.Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.LastMessageAt)
                    .ToListAsync();
            var mapper = _mapper.Map<List<ConversationVm>>(list);
            return ("", mapper);
        }
        public async Task<List<MessageVm>> GetMessagesByConversation(string conversationId)
        {
            var list = await _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
            var mapper = _mapper.Map<List<MessageVm>>(list);
            return mapper;

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
                    MaxQuestionsPerDay = 25,
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

        public async Task<string> DeleteConversation(string conversationId)
        {
            var conversation = await _context.Conversations.FirstOrDefaultAsync(x => x.ConversationId == conversationId);
            if (conversation == null) return "Conversation not found!";
            _context.Conversations.Remove(conversation);
            await _context.SaveChangesAsync();
            return "";
        }

        public async Task<string> RenameConversation(string conversationId, string title)
        {
            var conversation = await _context.Conversations.FirstOrDefaultAsync(x => x.ConversationId == conversationId);
            if (conversation == null) return "Conversation not found!";

            conversation.Title = title;
            conversation.UpdatedAt = DateTime.UtcNow;
            _context.Conversations.Update(conversation);
            await _context.SaveChangesAsync();
            return "";
        }


    }
}
