using System;

namespace TeamBuilder.Services.Core.Contracts.Team.Requests
{
    public class EventUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
} 