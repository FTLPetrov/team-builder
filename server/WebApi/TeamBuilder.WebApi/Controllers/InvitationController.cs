using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Services.Core.Contracts.Invitation.Requests;
using TeamBuilder.Services.Core.Contracts.Invitation.Responses;

namespace TeamBuilder.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<InvitationResponse>>> GetUserInvitations()
        {
            // Get current user from JWT token
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            
            // Debug logging
            Console.WriteLine($"User claims count: {User.Claims.Count()}");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
            }
            
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                Console.WriteLine($"UserIdClaim is null: {userIdClaim == null}");
                if (userIdClaim != null)
                {
                    Console.WriteLine($"UserIdClaim value: {userIdClaim.Value}");
                    Console.WriteLine($"Can parse as Guid: {Guid.TryParse(userIdClaim.Value, out _)}");
                }
                return Unauthorized();
            }
            
            Console.WriteLine($"Extracted userId: {userId}");
            var invitations = await _invitationService.GetUserInvitationsAsync(userId);
            Console.WriteLine($"Found {invitations.Count()} invitations for user {userId}");
            return Ok(invitations);
        }

        [HttpGet("test-auth")]
        public ActionResult<string> TestAuth()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var emailClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Email);
            var nameClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Name);
            
            return Ok(new
            {
                IsAuthenticated = User.Identity?.IsAuthenticated,
                UserId = userIdClaim?.Value,
                Email = emailClaim?.Value,
                Name = nameClaim?.Value,
                ClaimsCount = User.Claims.Count()
            });
        }

        [HttpGet("debug/all")]
        public async Task<ActionResult<IEnumerable<InvitationResponse>>> GetAllInvitations()
        {
            // This endpoint is for debugging - shows all invitations regardless of user
            var allInvitations = await _invitationService.GetAllAsync(Guid.Empty);
            return Ok(allInvitations);
        }

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