using System;

namespace TeamBuilder.Services.Core.Contracts.Team.Requests
{
    public class EventCreateRequest
    {
        public Guid TeamId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Location { get; set; }
        public Guid CreatedBy { get; set; }
    }
} 