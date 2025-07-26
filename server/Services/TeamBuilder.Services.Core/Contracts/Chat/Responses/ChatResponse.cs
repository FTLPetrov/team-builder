using System;

namespace TeamBuilder.Services.Core.Contracts.Chat.Responses
{
    public class ChatResponse
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public Guid SenderId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
} 