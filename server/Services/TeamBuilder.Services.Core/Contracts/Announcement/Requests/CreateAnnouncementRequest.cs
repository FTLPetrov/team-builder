namespace TeamBuilder.Services.Core.Contracts.Announcement.Requests
{
    public class CreateAnnouncementRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
} 