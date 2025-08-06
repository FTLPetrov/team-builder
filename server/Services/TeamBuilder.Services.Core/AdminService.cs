using Microsoft.EntityFrameworkCore;
using TeamBuilder.Data;
using TeamBuilder.Data.Models;
using TeamBuilder.Services.Core.Contracts.User.Responses;
using TeamBuilder.Services.Core.Contracts.Team.Responses;
using TeamBuilder.Services.Core.Contracts.SupportMessage.Responses;
using TeamBuilder.Services.Core.Contracts.Announcement.Responses;
using TeamBuilder.Services.Core.Contracts.Announcement.Requests;
using TeamBuilder.Services.Core.Contracts.User.Requests;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Data.Repositories;
using TeamBuilder.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace TeamBuilder.Services.Core
{
    public class AdminService : IAdminService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IChatRepository _chatRepository;
        private readonly ISupportMessageRepository _supportMessageRepository;
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly IWarningRepository _warningRepository;
        private readonly UserManager<User> _userManager;

        public AdminService(
            ITeamRepository teamRepository,
            ITeamMemberRepository teamMemberRepository,
            IEventRepository eventRepository,
            IChatRepository chatRepository,
            ISupportMessageRepository supportMessageRepository,
            IAnnouncementRepository announcementRepository,
            IWarningRepository warningRepository,
            UserManager<User> userManager)
        {
            _teamRepository = teamRepository;
            _teamMemberRepository = teamMemberRepository;
            _eventRepository = eventRepository;
            _chatRepository = chatRepository;
            _supportMessageRepository = supportMessageRepository;
            _announcementRepository = announcementRepository;
            _warningRepository = warningRepository;
            _userManager = userManager;
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList(); // Include all users including deleted ones
            
            return users.Select(u => new UserResponse
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                ProfilePictureUrl = u.ProfilePictureUrl,
                IsAdmin = u.IsAdmin,
                EmailConfirmed = u.EmailConfirmed,
                IsDeleted = u.IsDeleted,
                CreatedAt = DateTime.UtcNow // Default value since User doesn't have CreatedAt
            });
        }

        public async Task<UserResponse> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return null;

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                IsAdmin = user.IsAdmin,
                EmailConfirmed = user.EmailConfirmed,
                IsDeleted = user.IsDeleted,
                CreatedAt = DateTime.UtcNow // Default value since User doesn't have CreatedAt
            };
        }

        public async Task<bool> UpdateUserPasswordAsync(Guid userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserDetailsAsync(Guid userId, string firstName, string lastName, string email, string userName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;
            user.UserName = userName;
            
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            user.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> RecoverUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            user.IsDeleted = false;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> WarnUserAsync(Guid userId, string warningMessage, Guid adminUserId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var admin = await _userManager.FindByIdAsync(adminUserId.ToString());
            
            if (user == null || admin == null) return false;


            var warning = new Warning(warningMessage, userId, adminUserId);
            await _warningRepository.AddAsync(warning);

            return true;
        }

        public async Task<IEnumerable<TeamResponse>> GetAllTeamsAsync()
        {
            var teams = await _teamRepository.GetAllAsync();
            
            return teams.Select(t => new TeamResponse
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                IsOpen = t.IsOpen,
                OrganizerId = t.OrganizerId,
                CreatedAt = DateTime.UtcNow, // Default value since Team doesn't have CreatedAt
                Members = t.Members?.Select(tm => new TeamMemberResponse
                {
                    UserId = tm.UserId,
                    UserName = tm.User?.UserName ?? "Unknown",
                    FirstName = tm.User?.FirstName ?? "Unknown",
                    LastName = tm.User?.LastName ?? "Unknown",
                    Email = tm.User?.Email ?? "Unknown",
                    Role = tm.Role.ToString(),
                    ProfilePictureUrl = tm.User?.ProfilePictureUrl
                }).ToList() ?? new List<TeamMemberResponse>(),
                Events = new List<EventResponse>() // Will be populated separately if needed
            });
        }

        public async Task<TeamResponse> GetTeamByIdAsync(Guid teamId)
        {
            var team = await _teamRepository.GetByIdWithMembersAndEventsAsync(teamId);
            if (team == null) return null;

            return new TeamResponse
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                IsOpen = team.IsOpen,
                OrganizerId = team.OrganizerId,
                CreatedAt = DateTime.UtcNow, // Default value since Team doesn't have CreatedAt
                Members = team.Members?.Select(tm => new TeamMemberResponse
                {
                    UserId = tm.UserId,
                    UserName = tm.User?.UserName ?? "Unknown",
                    FirstName = tm.User?.FirstName ?? "Unknown",
                    LastName = tm.User?.LastName ?? "Unknown",
                    Email = tm.User?.Email ?? "Unknown",
                    Role = tm.Role.ToString(),
                    ProfilePictureUrl = tm.User?.ProfilePictureUrl
                }).ToList() ?? new List<TeamMemberResponse>(),
                Events = team.Events?.Select(e => new EventResponse
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    Date = e.Date,
                    CreatedBy = e.CreatedBy
                }).ToList() ?? new List<EventResponse>()
            };
        }

        public async Task<bool> DeleteTeamAsync(Guid teamId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null) return false;

            await _teamRepository.DeleteAsync(team);
            return true;
        }

        public async Task<bool> JoinTeamAsync(Guid teamId, Guid userId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            var user = await _userManager.FindByIdAsync(userId.ToString());
            
            if (team == null || user == null) return false;

            var existingMember = await _teamMemberRepository.GetByTeamAndUserAsync(teamId, userId);

            if (existingMember != null) return true; // Already a member

            var teamMember = new TeamMember
            {
                TeamId = teamId,
                UserId = userId,
                Role = TeamRole.Member
            };
            await _teamMemberRepository.AddAsync(teamMember);
            return true;
        }

        public async Task<bool> KickUserFromTeamAsync(Guid teamId, Guid userId)
        {
            var teamMember = await _teamMemberRepository.GetByTeamAndUserAsync(teamId, userId);

            if (teamMember == null) return false;

            await _teamMemberRepository.DeleteAsync(teamMember);
            return true;
        }

        public async Task<bool> ClearTeamChatAsync(Guid teamId)
        {
            var chats = await _chatRepository.FindAsync(c => c.TeamId == teamId);
            
            await _chatRepository.DeleteRangeAsync(chats);
            return true;
        }

        public async Task<IEnumerable<EventResponse>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllAsync();
            
            return events.Select(e => new EventResponse
            {
                Id = e.Id,
                Title = e.Name, // Map Name to Title
                Name = e.Name,
                Description = e.Description,
                Date = e.Date,
                StartDate = e.Date, // Use Date as StartDate
                EndDate = e.Date, // Use Date as EndDate for now
                Location = e.Location,
                CreatedBy = e.CreatedBy,
                TeamId = e.TeamId,
                TeamName = e.Team?.Name ?? "Unknown",
                CreatedAt = DateTime.UtcNow, // Default value since Event doesn't have CreatedAt
                Team = null, // Will be populated separately if needed
                Organizer = null, // Will be populated separately if needed
                ParticipatingTeams = null // Will be populated separately if needed
            });
        }

        public async Task<EventResponse> GetEventByIdAsync(Guid eventId)
        {
            var eventItem = await _eventRepository.GetByIdWithTeamAsync(eventId);
            if (eventItem == null) return null;

            return new EventResponse
            {
                Id = eventItem.Id,
                Title = eventItem.Name, // Map Name to Title
                Name = eventItem.Name,
                Description = eventItem.Description,
                Date = eventItem.Date,
                StartDate = eventItem.Date, // Use Date as StartDate
                EndDate = eventItem.Date, // Use Date as EndDate for now
                Location = eventItem.Location,
                CreatedBy = eventItem.CreatedBy,
                TeamId = eventItem.TeamId,
                TeamName = eventItem.Team?.Name ?? "Unknown",
                CreatedAt = DateTime.UtcNow, // Default value since Event doesn't have CreatedAt
                Team = null, // Will be populated separately if needed
                Organizer = null, // Will be populated separately if needed
                ParticipatingTeams = null // Will be populated separately if needed
            };
        }

        public async Task<bool> DeleteEventAsync(Guid eventId)
        {
            var eventItem = await _eventRepository.GetByIdAsync(eventId);
            if (eventItem == null) return false;

            await _eventRepository.DeleteAsync(eventItem);
            return true;
        }

        public async Task<IEnumerable<SupportMessageResponse>> GetAllSupportMessagesAsync()
        {
            var messages = await _supportMessageRepository.GetActiveMessagesAsync();
            
            return messages.Select(sm => new SupportMessageResponse
            {
                Id = sm.Id,
                Subject = sm.Subject,
                Message = sm.Message,
                UserId = sm.UserId,
                UserEmail = sm.User?.Email ?? "Unknown",
                UserName = sm.User?.UserName ?? "Unknown",
                UserFirstName = sm.User?.FirstName ?? "Unknown",
                UserLastName = sm.User?.LastName ?? "Unknown",
                CreatedAt = sm.CreatedAt,
                IsRead = sm.IsRead,
                IsFavorite = sm.IsFavorite,
                IsCompleted = sm.IsCompleted
            });
        }

        public async Task<SupportMessageResponse> GetSupportMessageByIdAsync(Guid messageId)
        {
            var message = await _supportMessageRepository.GetByIdAsync(messageId);
            if (message == null || message.IsDeleted) return null;

            return new SupportMessageResponse
            {
                Id = message.Id,
                Subject = message.Subject,
                Message = message.Message,
                UserId = message.UserId,
                UserEmail = message.User?.Email ?? "Unknown",
                UserName = message.User?.UserName ?? "Unknown",
                UserFirstName = message.User?.FirstName ?? "Unknown",
                UserLastName = message.User?.LastName ?? "Unknown",
                CreatedAt = message.CreatedAt,
                IsRead = message.IsRead,
                IsFavorite = message.IsFavorite,
                IsCompleted = message.IsCompleted
            };
        }

        public async Task<bool> MarkSupportMessageAsReadAsync(Guid messageId)
        {
            var message = await _supportMessageRepository.GetByIdAsync(messageId);
            if (message == null) return false;

            message.IsRead = true;
            await _supportMessageRepository.UpdateAsync(message);
            return true;
        }

        public async Task<bool> ToggleSupportMessageFavoriteAsync(Guid messageId)
        {
            var message = await _supportMessageRepository.GetByIdAsync(messageId);
            if (message == null) return false;

            message.IsFavorite = !message.IsFavorite;
            await _supportMessageRepository.UpdateAsync(message);
            return true;
        }

        public async Task<bool> MarkSupportMessageAsCompletedAsync(Guid messageId)
        {
            var message = await _supportMessageRepository.GetByIdAsync(messageId);
            if (message == null) return false;

            message.IsCompleted = !message.IsCompleted;
            await _supportMessageRepository.UpdateAsync(message);
            return true;
        }

        public async Task<bool> DeleteSupportMessageAsync(Guid messageId)
        {
            var message = await _supportMessageRepository.GetByIdAsync(messageId);
            if (message == null) return false;

            message.IsDeleted = true;
            await _supportMessageRepository.UpdateAsync(message);
            return true;
        }


        public async Task<AnnouncementResponse> CreateAnnouncementAsync(CreateAnnouncementRequest request, Guid adminUserId)
        {
            var admin = await _userManager.FindByIdAsync(adminUserId.ToString());
            if (admin == null || !admin.IsAdmin) 
                throw new InvalidOperationException("Only admins can create announcements");

            var announcement = new Announcement(request.Title, request.Message, adminUserId);
            await _announcementRepository.AddAsync(announcement);

            return new AnnouncementResponse
            {
                Id = announcement.Id,
                Title = announcement.Title,
                Message = announcement.Message,
                CreatedByUserName = $"{admin.FirstName} {admin.LastName}",
                CreatedAt = announcement.CreatedAt,
                IsActive = announcement.IsActive
            };
        }

        public async Task<IEnumerable<AnnouncementResponse>> GetAllAnnouncementsAsync()
        {
            var announcements = await _announcementRepository.GetAllAnnouncementsAsync();

            return announcements.Select(a => new AnnouncementResponse
            {
                Id = a.Id,
                Title = a.Title,
                Message = a.Message,
                CreatedByUserName = $"{a.CreatedByUser.FirstName} {a.CreatedByUser.LastName}",
                CreatedAt = a.CreatedAt,
                IsActive = a.IsActive
            });
        }

        public async Task<IEnumerable<AnnouncementResponse>> GetAllAnnouncementsForAdminAsync()
        {
            var announcements = await _announcementRepository.GetAllAnnouncementsAsync();

            return announcements.Select(a => new AnnouncementResponse
            {
                Id = a.Id,
                Title = a.Title,
                Message = a.Message,
                CreatedByUserName = $"{a.CreatedByUser.FirstName} {a.CreatedByUser.LastName}",
                CreatedAt = a.CreatedAt,
                IsActive = a.IsActive
            });
        }

        public async Task<bool> DeleteAnnouncementAsync(Guid announcementId)
        {
            var announcement = await _announcementRepository.GetByIdAsync(announcementId);
            if (announcement == null) return false;

            announcement.IsDeleted = true;
            await _announcementRepository.UpdateAsync(announcement);
            return true;
        }

        public async Task<bool> ToggleAnnouncementActiveAsync(Guid announcementId)
        {
            var announcement = await _announcementRepository.GetByIdAsync(announcementId);
            if (announcement == null) return false;

            announcement.IsActive = !announcement.IsActive;
            await _announcementRepository.UpdateAsync(announcement);
            return true;
        }

        public async Task<bool> ActivateAllAnnouncementsAsync()
        {
            var announcements = await _announcementRepository.GetAllAnnouncementsAsync();
            foreach (var announcement in announcements)
            {
                announcement.IsActive = true;
                await _announcementRepository.UpdateAsync(announcement);
            }
            return true;
        }


        public async Task<WarningResponse> CreateWarningAsync(CreateWarningRequest request, Guid userId, Guid adminUserId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var admin = await _userManager.FindByIdAsync(adminUserId.ToString());
            
            if (user == null || admin == null || !admin.IsAdmin)
                throw new InvalidOperationException("Invalid user or admin not found");

            var warning = new Warning(request.Message, userId, adminUserId);
            await _warningRepository.AddAsync(warning);

            return new WarningResponse
            {
                Id = warning.Id,
                Message = warning.Message,
                UserId = warning.UserId,
                UserName = $"{user.FirstName} {user.LastName}",
                UserEmail = user.Email,
                CreatedByUserName = $"{admin.FirstName} {admin.LastName}",
                CreatedAt = warning.CreatedAt
            };
        }

        public async Task<IEnumerable<WarningResponse>> GetAllWarningsAsync()
        {
            var warnings = await _warningRepository.GetActiveWarningsAsync();

            return warnings.Select(w => new WarningResponse
            {
                Id = w.Id,
                Message = w.Message,
                UserId = w.UserId,
                UserName = w.User != null ? $"{w.User.FirstName} {w.User.LastName}" : "Unknown User",
                UserEmail = w.User?.Email ?? "Unknown Email",
                CreatedByUserName = w.CreatedByUser != null ? $"{w.CreatedByUser.FirstName} {w.CreatedByUser.LastName}" : "Unknown Admin",
                CreatedAt = w.CreatedAt
            });
        }

        public async Task<bool> DeleteWarningAsync(Guid warningId)
        {
            var warning = await _warningRepository.GetByIdAsync(warningId);
            if (warning == null) return false;

            warning.IsDeleted = true;
            await _warningRepository.UpdateAsync(warning);
            return true;
        }
    }
} 