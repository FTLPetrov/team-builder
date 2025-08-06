using Microsoft.AspNetCore.Mvc;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Services.Core.Contracts.Team.Requests;
using TeamBuilder.Services.Core.Contracts.Team.Responses;

namespace TeamBuilder.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventResponse>>> GetAll()
            => Ok(await _eventService.GetAllAsync());

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<EventResponse>>> GetUserEvents()
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj == null)
            {
                return Unauthorized();
            }
            var userId = (Guid)userIdObj;
            var userEvents = await _eventService.GetUserEventsAsync(userId);
            return Ok(userEvents);
        }

        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<IEnumerable<EventResponse>>> GetTeamEvents(Guid teamId)
            => Ok(await _eventService.GetAllAsync(teamId));

        [HttpGet("{eventId}")]
        public async Task<ActionResult<EventResponse>> GetById(Guid eventId)
        {
            var ev = await _eventService.GetByIdAsync(eventId);
            if (ev == null) return NotFound();
            return Ok(ev);
        }

        [HttpPost]
        public async Task<ActionResult<EventCreateResponse>> Create([FromBody] EventCreateRequest dto)
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj == null)
            {
                return Unauthorized();
            }
            var userId = (Guid)userIdObj;
            dto.CreatedBy = userId;
            

            var team = await _eventService.GetTeamByIdAsync(dto.TeamId);
            if (team == null)
            {
                return NotFound(new { message = "Team not found" });
            }
            
            if (team.OrganizerId != userId)
            {
                return StatusCode(403, new { message = "Only team organizers can create events for the team" });
            }
            
            var result = await _eventService.CreateAsync(dto);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { eventId = result.Id }, result);
        }

        [HttpPut("{eventId}")]
        public async Task<ActionResult<EventUpdateResponse>> Update(Guid eventId, [FromBody] EventUpdateRequest dto)
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj == null)
            {
                return Unauthorized();
            }
            var userId = (Guid)userIdObj;
            

            var existingEvent = await _eventService.GetByIdAsync(eventId);
            if (existingEvent == null) return NotFound();
            

            bool isAuthorized = existingEvent.CreatedBy == userId || 
                               (existingEvent.Team != null && existingEvent.Team.OrganizerId == userId);
            
            if (!isAuthorized) return Forbid();
            
            var result = await _eventService.UpdateAsync(eventId, dto);
            if (result == null) return NotFound();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{eventId}")]
        public async Task<IActionResult> Delete(Guid eventId)
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj == null)
            {
                return Unauthorized();
            }
            var userId = (Guid)userIdObj;
            

            var existingEvent = await _eventService.GetByIdAsync(eventId);
            if (existingEvent == null) return NotFound();
            

            bool isAuthorized = existingEvent.CreatedBy == userId || 
                               (existingEvent.Team != null && existingEvent.Team.OrganizerId == userId);
            
            if (!isAuthorized) return Forbid();
            
            var deleted = await _eventService.DeleteAsync(eventId);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
} 