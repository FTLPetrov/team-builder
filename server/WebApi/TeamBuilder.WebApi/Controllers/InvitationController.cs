using Microsoft.AspNetCore.Mvc;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Services.Core.Contracts.Invitation.Requests;
using TeamBuilder.Services.Core.Contracts.Invitation.Responses;

namespace TeamBuilder.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationController : ControllerBase
    {
        private readonly IInvitationService _invitationService;
        public InvitationController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<IEnumerable<InvitationResponse>>> GetAll(Guid teamId)
            => Ok(await _invitationService.GetAllAsync(teamId));

        [HttpGet("{invitationId}")]
        public async Task<ActionResult<InvitationResponse>> GetById(Guid invitationId)
        {
            var inv = await _invitationService.GetByIdAsync(invitationId);
            if (inv == null) return NotFound();
            return Ok(inv);
        }

        [HttpPost]
        public async Task<ActionResult<InvitationCreateResponse>> Create([FromBody] InvitationCreateRequest dto)
        {
            var result = await _invitationService.CreateAsync(dto);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { invitationId = result.Id }, result);
        }

        [HttpPost("respond")]
        public async Task<ActionResult<InvitationRespondResponse>> Respond([FromBody] InvitationRespondRequest dto)
        {
            var result = await _invitationService.RespondAsync(dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{invitationId}")]
        public async Task<IActionResult> Delete(Guid invitationId)
        {
            var deleted = await _invitationService.DeleteAsync(invitationId);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
} 