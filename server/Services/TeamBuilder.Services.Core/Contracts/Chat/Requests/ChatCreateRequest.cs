using System;

namespace TeamBuilder.Services.Core.Contracts.Chat.Requests
{
    public sealed class ChatCreateRequest
    {
        public Guid TeamId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
} 