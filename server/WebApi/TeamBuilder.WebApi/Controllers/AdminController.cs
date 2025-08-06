using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Services.Core.Contracts.User.Responses;
using TeamBuilder.Services.Core.Contracts.User.Requests;
using TeamBuilder.Services.Core.Contracts.Team.Responses;
using TeamBuilder.Services.Core.Contracts.SupportMessage.Responses;
using TeamBuilder.Services.Core.Contracts.Announcement.Responses;
using TeamBuilder.Services.Core.Contracts.Announcement.Requests;

namespace TeamBuilder.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService, IValidationService validationService) 
            : base(validationService)
        {
            _adminService = adminService;
        }


        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("users/{userId}")]
        public async Task<ActionResult<UserResponse>> GetUserById(Guid userId)
        {
            var user = await _adminService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("users/{userId}/password")]
        public async Task<ActionResult> UpdateUserPassword(Guid userId, [FromBody] string newPassword)
        {
            var success = await _adminService.UpdateUserPasswordAsync(userId, newPassword);
            if (!success)
                return NotFound();

            return Ok();
        }

        [HttpPut("users/{userId}/details")]
        public async Task<ActionResult> UpdateUserDetails(Guid userId, [FromBody] UserUpdateRequest request)
        {
            var success = await _adminService.UpdateUserDetailsAsync(userId, request.FirstName, request.LastName, request.Email, request.UserName);
            if (!success)
                return NotFound();

            return Ok();
        }

        [HttpDelete("users/{userId}")]
        public async Task<ActionResult> DeleteUser(Guid userId)
        {
            var success = await _adminService.DeleteUserAsync(userId);
            if (!success)
                return NotFound();

            return Ok();
        }

        [HttpPut("users/{userId}/recover")]
        public async Task<ActionResult> RecoverUser(Guid userId)
        {
            var success = await _adminService.RecoverUserAsync(userId);
            if (!success)
                return NotFound();

            return Ok();
        }

        [HttpPost("users/{userId}/warn")]
        public async Task<ActionResult> WarnUser(Guid userId, [FromBody] string warningMessage)
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var adminUserIdObj) || adminUserIdObj is not Guid adminUserId)
            {
                return Unauthorized();
            }

            var success = await _adminService.WarnUserAsync(userId, warningMessage, adminUserId);
            if (!success)
                return NotFound();

            return Ok();
        }


        [HttpGet("teams")]
        public async Task<ActionResult<IEnumerable<TeamResponse>>> GetAllTeams()
        {
            var teams = await _adminService.GetAllTeamsAsync();
            return Ok(teams);
        }

        [HttpGet("teams/{teamId}")]
        public async Task<ActionResult<TeamResponse>> GetTeamById(Guid teamId)
        {
            var team = await _adminService.GetTeamByIdAsync(teamId);
            if (team == null)
                return NotFound();

            return Ok(team);
        }

        [HttpDelete("teams/{teamId}")]
        public async Task<ActionResult> DeleteTeam(Guid teamId)
        {
            var success = await _adminService.DeleteTeamAsync(teamId);
            if (!success)
                return NotFound();

            return Ok();
        }

        [HttpPost("teams/{teamId}/join/{userId}")]
        public async Task<ActionResult> JoinTeam(Guid teamId, Guid userId)
        {
            var success = await _adminService.JoinTeamAsync(teamId, userId);
            if (!success)
                return NotFound();

            return Ok();
        }

        [HttpDelete("teams/{teamId}/members/{userId}")]
        public async Task<ActionResult> KickUserFromTeam(Guid teamId, Guid userId)
        {
            var success = await _adminService.KickUserFromTeamAsync(teamId, userId);
            if (!success)
                return NotFound();

            return Ok();
        }

        [HttpDelete("teams/{teamId}/chat")]
        public async Task<ActionResult> ClearTeamChat(Guid teamId)
        {
            var success = await _adminService.ClearTeamChatAsync(teamId);
            if (!success)
                return NotFound();

            return Ok();
        }


        [HttpGet("events")]
        public async Task<ActionResult<IEnumerable<EventResponse>>> GetAllEvents()
        {
            var events = await _adminService.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpGet("events/{eventId}")]
        public async Task<ActionResult<EventResponse>> GetEventById(Guid eventId)
        {
            var eventItem = await _adminService.GetEventByIdAsync(eventId);
            if (eventItem == null)
                return NotFound();

            return Ok(eventItem);
        }

        [HttpDelete("events/{eventId}")]
        public async Task<ActionResult> DeleteEvent(Guid eventId)
        {
            var success = await _adminService.DeleteEventAsync(eventId);
            if (!success)
                return NotFound();

            return Ok();
        }


        [HttpGet("support-messages")]
        public async Task<ActionResult<IEnumerable<SupportMessageResponse>>> GetAllSupportMessages()
        {
            var messages = await _adminService.GetAllSupportMessagesAsync();
            return Ok(messages);
        }

        [HttpGet("support-messages/{messageId}")]
        public async Task<ActionResult<SupportMessageResponse>> GetSupportMessageById(Guid messageId)
        {
            var message = await _adminService.GetSupportMessageByIdAsync(messageId);
            if (message == null)
                return NotFound();

            return Ok(message);
        }

        [HttpPut("support-messages/{messageId}/read")]
        public async Task<ActionResult> MarkSupportMessageAsRead(Guid messageId)
        {
            var success = await _adminService.MarkSupportMessageAsReadAsync(messageId);
            if (!success)
                return NotFound();

            return Ok();
        }

        [HttpPut("support-messages/{messageId}/favorite")]
        public async Task<ActionResult> ToggleSupportMessageFavorite(Guid messageId)
        {
            var success = await _adminService.ToggleSupportMessageFavoriteAsync(messageId);
            if (!success)
                return NotFound();

            return Ok();
        }

        [HttpPut("support-messages/{messageId}/completed")]
        public async Task<ActionResult> MarkSupportMessageAsCompleted(Guid messageId)
        {
            var success = await _adminService.MarkSupportMessageAsCompletedAsync(messageId);
            if (!success)
                return NotFound();

            return Ok();
        }

        [HttpDelete("support-messages/{messageId}")]
        public async Task<ActionResult> DeleteSupportMessage(Guid messageId)
        {
            var success = await _adminService.DeleteSupportMessageAsync(messageId);
            if (!success)
                return NotFound();

            return Ok();
        }


        [HttpPost("announcements")]
        public async Task<ActionResult<AnnouncementResponse>> CreateAnnouncement([FromBody] CreateAnnouncementRequest request)
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is not Guid userId)
            {
                return Unauthorized();
            }

            try
            {
                var announcement = await _adminService.CreateAnnouncementAsync(request, userId);
                return CreatedAtAction(nameof(GetAllAnnouncements), announcement);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("announcements")]
        public async Task<ActionResult<IEnumerable<AnnouncementResponse>>> GetAllAnnouncements()
        {
            var announcements = await _adminService.GetAllAnnouncementsForAdminAsync();
            return Ok(announcements);
        }

        [HttpDelete("announcements/{announcementId}")]
        public async Task<ActionResult> DeleteAnnouncement(Guid announcementId)
        {
            var success = await _adminService.DeleteAnnouncementAsync(announcementId);
            if (!success)
                return NotFound();

            return Ok();
        }

        [HttpPut("announcements/{announcementId}/toggle")]
        public async Task<ActionResult> ToggleAnnouncementActive(Guid announcementId)
        {
            var success = await _adminService.ToggleAnnouncementActiveAsync(announcementId);
            if (!success)
                return NotFound();

            return Ok();
        }


        [HttpPost("users/{userId}/warnings")]
        public async Task<ActionResult<WarningResponse>> CreateWarning(Guid userId, [FromBody] CreateWarningRequest request)
        {
            return await ValidateAndExecuteAsync(request, async (validatedRequest) =>
            {
                if (!HttpContext.Items.TryGetValue("UserId", out var adminUserIdObj) || adminUserIdObj is not Guid adminUserId)
                {
                    return UnauthorizedResponse("No valid admin user found");
                }

                try
                {
                    var warning = await _adminService.CreateWarningAsync(validatedRequest, userId, adminUserId);
                    return CreatedAtAction(nameof(GetAllWarnings), warning);
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequestResponse(ex.Message);
                }
            });
        }

        [HttpGet("warnings")]
        public async Task<ActionResult<IEnumerable<WarningResponse>>> GetAllWarnings()
        {
            try
            {
                var warnings = await _adminService.GetAllWarningsAsync();
                return Ok(warnings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("warnings/{warningId}")]
        public async Task<ActionResult> DeleteWarning(Guid warningId)
        {
            var success = await _adminService.DeleteWarningAsync(warningId);
            if (!success)
                return NotFound();

            return Ok();
        }


        [HttpPost("announcements/activate-all")]
        public async Task<ActionResult> ActivateAllAnnouncements()
        {
            var success = await _adminService.ActivateAllAnnouncementsAsync();
            return Ok(new { message = "All announcements activated" });
        }
    }
} 