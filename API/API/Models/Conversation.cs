using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Conversation
{
    public string ConversationId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastMessageAt { get; set; }

    public string? ModelUsed { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual User User { get; set; } = null!;
}
