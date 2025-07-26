using System;

namespace TeamBuilder.Services.Core.Contracts.Team.Responses
{
    public class EventCreateResponse
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public Guid CreatedBy { get; set; }
    }
} 