namespace API.ViewModels
{
    public class MessageVm
    {
        public string MessageId { get; set; }  
        public string ConversationId { get; set; }      
        public int Role { get; set; }
        public string Content { get; set; } 
        public DateTime? CreatedAt { get; set; }
    }

    public class ConversationVm
    {
        public string ConversationId { get; set; }  
        public string Title { get; set; } 
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }
    }
}
