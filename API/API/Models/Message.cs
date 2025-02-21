using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Message
{
    public string MessageId { get; set; } = null!;

    public string ConversationId { get; set; } = null!;

    public int Role { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;
}
