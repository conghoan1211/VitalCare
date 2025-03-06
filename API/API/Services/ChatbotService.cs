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
        private readonly string AIUri = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-pro-exp-02-05:generateContent";

        private readonly string InitialSystemPrompt = @"Bạn là trợ lý AI của VitalCare, một nền tảng chuyên cung cấp thông tin và dịch vụ về sức khỏe xương khớp. Nhiệm vụ của bạn là hướng dẫn người dùng sử dụng website, tư vấn về các vấn đề xương khớp, và giúp họ tiếp cận thông tin một cách nhanh chóng, cô đọng và dễ hiểu. Khi tư vấn về sức khỏe, hãy ưu tiên các giải pháp tự nhiên, bài tập hỗ trợ và khuyến khích người dùng tham khảo ý kiến bác sĩ khi cần thiết.";
        private readonly string SecondarySystemPrompt = @"Khi trả lời về các phương pháp điều trị, bạn cần tuân theo các nguyên tắc sau:
                                    - Luôn dựa trên bằng chứng khoa học và nghiên cứu y khoa cập nhật
                                    - Ưu tiên đề cập đến các phương pháp điều trị đã được chứng minh hiệu quả
                                    - Giải thích rõ ràng về cơ chế tác động và lợi ích của từng phương pháp
                                    - Cảnh báo về các tác dụng phụ có thể xảy ra
                                    - Nhấn mạnh tầm quan trọng của việc tuân thủ phác đồ điều trị
                                \n Bạn là trợ lý AI của VitalCare, trang web vitalcare có cung cấp các sản phẩm để giúp người dùng cải thiện bệnh cơ xương khớp, và sản phẩm chủ yếu sẽ là sữa dinh dưỡng, miếng dán giảm đau và các thực phẩm dinh dưỡng khác
                              Ngoài ra trang web còn có các bài viết, video luyện tập bổ ích cho việc cải thiện sức khỏe cơ xương khớp. \n
                              khi người dùng hỏi 1 sản phẩm cụ thể nào đó thì hãy bảo họ vao trực tiếp trang danh sách sản phẩm hoặc ô tìm kiếm sản phẩm để xem.,
                              thậm chỉ là sản phẩm đó nếu ko có, thì bảo họ tham khảo các sản phẩm khác.\n

                              + Các câu trả lời của bạn phải hạn chế việc sử dụng chữ đậm hoặc nghiêng. \n
                              + Các câu trả lời của bạn chỉ được tối đa 600 token\n
                              + Câu trả lời cần phải chọn vẹn, không cần quá dài nhưng cần đầy đủ, cô đọng, không được ngắt quãng\n.";

        private readonly string UseSystemPrompt = @"Hướng Dẫn Sử Dụng Website 💡 Cách đăng ký tài khoản & đăng nhập:
                                            Khi người dùng hỏi về cách đăng ký tài khoản trên trang web VitalCare, hãy hướng dẫn họ từng bước:
                                            Cách 1:
                                            1️⃣ Nhấn vào nút 'Đăng ký' ở góc trên cùng bên phải.
                                            2️⃣ Nhập họ và tên, email và tạo mật khẩu.
                                            3️⃣ Xác nhận tài khoản qua email bằng mã OTP được gửi vào địa chỉ email.
                                            4️⃣ Đăng nhập bằng tài khoản vừa tạo.""
                                            Cách 2:
                                            đăng kí trực tiếp vào nút đăng nhập bằng google

                                            💡 Cách đặt hàng & thanh toán:
                                            Khi người dùng hỏi về cách mua hàng, hãy hướng dẫn họ chi tiết:
                                            1️⃣ Chọn sản phẩm muốn mua và nhấn 'Thêm vào giỏ hàng'.
                                            2️⃣ Vào giỏ hàng, kiểm tra sản phẩm, số lượng.
                                            3️⃣ Nhập địa chỉ nhận hàng, chọn phương thức thanh toán.
                                            4️⃣ Nhấn 'Xác nhận đơn hàng' để hoàn tất.

                                        \n cách cập nhật thông tin cá nhân hay đổi mật khẩu, theo dõi đơn hàng, hãy bảo họ vào trang profile, ấn vào hình avatar ở góc trên bên phải để vào và thực hiện";



        private readonly string ImportantSystemPrompt = @"Sau mỗi khi đưa ra các phương pháp điều trị hay các tư vấn, bạn hãy khuyến khích người dùng tham khảo các bài viết, các sản phẩm hoặc video luyện tập ở ngay trên web VitalCare của chúng ta;
                                           bạn có thể trả lời câu hỏi nằm ngoài phạm vi chăm sóc sức khỏe hay vitalcare nhưng không được đi quá xa, nên nói cho người dùng biết nếu cuộc trò chuyện đang đi quá xa. \n
                                           - Ưu tiên: Các vấn đề liên quan đến VitalCare và chăm sóc sức khỏe xương khớp.
                                           - Mở rộng: Có thể trả lời các câu hỏi nằm ngoài phạm vi trên, nhưng chỉ ở mức độ vừa phải, không đi quá sâu vào chi tiết.
                                           - Cảnh báo: Nếu cuộc trò chuyện bắt đầu đi quá xa khỏi chủ đề sức khỏe và VitalCare, tôi sẽ nhắc nhở người dùng và hướng họ trở lại các chủ đề phù hợp.
                                 
                                            \nKhi người dùng hỏi ai đã sáng lập hay phát triển ra website VitalCare. Bạn cần phải trả lời là do 1 nhóm sinh viên trường đại học FPT phát triển.\n
                                            trong đó bên Marketing, nghiên cứu thị trường là các bạn: Lê Nguyễn Tùng Dương, Lương Tuệ Quang, Nguyễn Trà My. Bên phát triển Web là : Phạm Công Hoan, Cao Trường Sơn, Chu Thiên Quân. ";

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

            // Kiểm tra xem hội thoại này đã có tin nhắn nào từ AI (model) chưa
            bool hasAIResponse = messages.Any(m => m.role == "model");

            // Nếu chưa có tin nhắn từ AI (tức là lần đầu mở), thêm 4 prompt hệ thống
            if (!hasAIResponse)
            {
                chatHistory.Add(new { role = "model", parts = new[] { new { text = InitialSystemPrompt } } });
                chatHistory.Add(new { role = "model", parts = new[] { new { text = SecondarySystemPrompt } } });
                chatHistory.Add(new { role = "model", parts = new[] { new { text = UseSystemPrompt } } });
                chatHistory.Add(new { role = "model", parts = new[] { new { text = ImportantSystemPrompt } } });
            }

            // Nếu có hơn 10 tin nhắn, chỉ lấy tin nhắn gần nhất của user
            if (messages.Count > 10)
            {
                var lastUserMessage = messages.LastOrDefault(m => m.role == "user");
                if (lastUserMessage != null)
                {
                    chatHistory.Add(new
                    {
                        role = "user",
                        parts = new[] { new { text = lastUserMessage.content } }
                    });
                }
            }
            else
            {
                // Nếu ít hơn 10 tin nhắn, lấy tất cả
                chatHistory.AddRange(messages.Select(m => new
                {
                    role = m.role,
                    parts = new[] { new { text = m.content } }
                }));
            }

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
                        temperature = 1.0,
                        topP = 0.7, // Giảm xuống
                        topK = 100, // Tăng lên
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
            conversation.UpdatedAt = DateTime.Now;
            _context.Conversations.Update(conversation);
            await _context.SaveChangesAsync();
            return "";
        }


    }
}
