using System;

namespace TeamBuilder.Services.Core.Contracts.Chat.Responses
{
    public class ChatCreateResponse
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public Guid? Id { get; set; }
    }
} 