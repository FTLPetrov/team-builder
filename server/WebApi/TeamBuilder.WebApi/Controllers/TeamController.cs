using Microsoft.AspNetCore.Mvc;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Services.Core.Contracts.Team.Requests;
using TeamBuilder.Services.Core.Contracts.Team.Responses;

namespace TeamBuilder.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;
        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamResponse>>> GetAll()
            => Ok(await _teamService.GetAllAsync());

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<TeamResponse>>> GetUserTeams()
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj == null)
            {
                return Unauthorized();
            }
            var userId = (Guid)userIdObj;
            var userTeams = await _teamService.GetUserTeamsAsync(userId);
            return Ok(userTeams);
        }

        [HttpPost("{id}/join")]
        public async Task<ActionResult> JoinTeam(Guid id)
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj == null)
            {
                return Unauthorized();
            }
            var userId = (Guid)userIdObj;
            var success = await _teamService.JoinTeamAsync(id, userId);
            if (success)
            {
                return Ok(new { success = true, message = "Successfully joined the team" });
            }
            return BadRequest(new { success = false, message = "Failed to join team. Team may be closed or you may already be a member." });
        }

        [HttpPost("{id}/leave")]
        public async Task<ActionResult> LeaveTeam(Guid id)
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj == null)
            {
                return Unauthorized();
            }
            var userId = (Guid)userIdObj;
            var success = await _teamService.LeaveTeamAsync(id, userId);
            if (success)
            {
                return Ok(new { success = true, message = "Successfully left the team" });
            }
            return BadRequest(new { success = false, message = "Failed to leave team. You may not be a member or you may be the organizer." });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamResponse>> GetById(Guid id)
        {
            var team = await _teamService.GetByIdAsync(id);
            if (team == null) return NotFound();
            return Ok(team);
        }

        [HttpPost]
        public async Task<ActionResult<TeamCreateResponse>> Create([FromBody] TeamCreateRequest dto)
        {
            var result = await _teamService.CreateAsync(dto);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TeamUpdateResponse>> Update(Guid id, [FromBody] TeamUpdateRequest dto)
        {
            var result = await _teamService.UpdateAsync(id, dto);
            if (result == null) return NotFound();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _teamService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPost("{teamId}/kick")]
        public async Task<IActionResult> KickMember(Guid teamId, [FromQuery] Guid userId)
        {
            var result = await _teamService.KickMemberAsync(teamId, userId);
            if (!result) return BadRequest();
            return Ok();
        }

        [HttpPost("{teamId}/assign-role")]
        public async Task<IActionResult> AssignRole(Guid teamId, [FromQuery] Guid userId, [FromQuery] string role)
        {
            var result = await _teamService.AssignRoleAsync(teamId, userId, role);
            if (!result) return BadRequest();
            return Ok();
        }

        [HttpPost("{teamId}/transfer-ownership")]
        public async Task<IActionResult> TransferOwnership(Guid teamId, [FromQuery] Guid newOrganizerId)
        {
            var result = await _teamService.TransferOwnershipAsync(teamId, newOrganizerId);
            if (!result) return BadRequest();
            return Ok();
        }
    }
} 