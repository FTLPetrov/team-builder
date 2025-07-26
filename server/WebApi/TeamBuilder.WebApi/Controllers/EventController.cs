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

        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<IEnumerable<EventResponse>>> GetAll(Guid teamId)
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
            var result = await _eventService.CreateAsync(dto);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { eventId = result.Id }, result);
        }

        [HttpPut("{eventId}")]
        public async Task<ActionResult<EventUpdateResponse>> Update(Guid eventId, [FromBody] EventUpdateRequest dto)
        {
            var result = await _eventService.UpdateAsync(eventId, dto);
            if (result == null) return NotFound();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{eventId}")]
        public async Task<IActionResult> Delete(Guid eventId)
        {
            var deleted = await _eventService.DeleteAsync(eventId);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
} 