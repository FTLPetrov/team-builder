using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamBuilder.Services.Core.Contracts.Announcement.Responses;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Data.Repositories.Interfaces;

namespace TeamBuilder.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnouncementRepository _announcementRepository;

        public AnnouncementController(IAnnouncementRepository announcementRepository)
        {
            _announcementRepository = announcementRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnnouncementResponse>>> GetActiveAnnouncements()
        {
            var announcements = await _announcementRepository.GetActiveAnnouncementsAsync();

            var response = announcements.Select(a => new AnnouncementResponse
            {
                Id = a.Id,
                Title = a.Title,
                Message = a.Message,
                CreatedByUserName = $"{a.CreatedByUser.FirstName} {a.CreatedByUser.LastName}",
                CreatedAt = a.CreatedAt,
                IsActive = a.IsActive
            });

            return Ok(response);
        }
    }
} 