using System;

namespace TeamBuilder.Services.Core.Contracts.Chat.Requests
{
    public class ChatCreateRequest
    {
        public Guid TeamId { get; set; }
        public Guid SenderId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
} 