namespace TeamBuilder.Services.Core.Contracts.Team.Requests
{
    public class TeamUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsOpen { get; set; }
    }
} 