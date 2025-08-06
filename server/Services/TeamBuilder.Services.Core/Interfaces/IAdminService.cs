using TeamBuilder.Services.Core.Contracts.User.Responses;
using TeamBuilder.Services.Core.Contracts.Team.Responses;
using TeamBuilder.Services.Core.Contracts.SupportMessage.Responses;
using TeamBuilder.Services.Core.Contracts.Announcement.Responses;
using TeamBuilder.Services.Core.Contracts.Announcement.Requests;
using TeamBuilder.Services.Core.Contracts.User.Requests;

namespace TeamBuilder.Services.Core.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<UserResponse> GetUserByIdAsync(Guid userId);
        Task<bool> UpdateUserPasswordAsync(Guid userId, string newPassword);
        Task<bool> UpdateUserDetailsAsync(Guid userId, string firstName, string lastName, string email, string userName);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<bool> RecoverUserAsync(Guid userId);
        Task<bool> WarnUserAsync(Guid userId, string warningMessage, Guid adminUserId);
        
        Task<IEnumerable<TeamResponse>> GetAllTeamsAsync();
        Task<TeamResponse> GetTeamByIdAsync(Guid teamId);
        Task<bool> DeleteTeamAsync(Guid teamId);
        Task<bool> JoinTeamAsync(Guid teamId, Guid userId);
        Task<bool> KickUserFromTeamAsync(Guid teamId, Guid userId);
        Task<bool> ClearTeamChatAsync(Guid teamId);
        
        Task<IEnumerable<EventResponse>> GetAllEventsAsync();
        Task<EventResponse> GetEventByIdAsync(Guid eventId);
        Task<bool> DeleteEventAsync(Guid eventId);
        
        Task<IEnumerable<SupportMessageResponse>> GetAllSupportMessagesAsync();
        Task<SupportMessageResponse> GetSupportMessageByIdAsync(Guid messageId);
        Task<bool> MarkSupportMessageAsReadAsync(Guid messageId);
        Task<bool> ToggleSupportMessageFavoriteAsync(Guid messageId);
        Task<bool> MarkSupportMessageAsCompletedAsync(Guid messageId);
        Task<bool> DeleteSupportMessageAsync(Guid messageId);

        // Announcement Management
        Task<AnnouncementResponse> CreateAnnouncementAsync(CreateAnnouncementRequest request, Guid adminUserId);
        Task<IEnumerable<AnnouncementResponse>> GetAllAnnouncementsAsync();
        Task<IEnumerable<AnnouncementResponse>> GetAllAnnouncementsForAdminAsync();
        Task<bool> DeleteAnnouncementAsync(Guid announcementId);
        Task<bool> ToggleAnnouncementActiveAsync(Guid announcementId);
        Task<bool> ActivateAllAnnouncementsAsync();

        // Warning Management
        Task<WarningResponse> CreateWarningAsync(CreateWarningRequest request, Guid userId, Guid adminUserId);
        Task<IEnumerable<WarningResponse>> GetAllWarningsAsync();
        Task<bool> DeleteWarningAsync(Guid warningId);
    }
} 