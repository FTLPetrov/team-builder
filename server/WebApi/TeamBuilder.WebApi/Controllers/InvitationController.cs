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

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            

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

        [HttpPost("by-email")]
        public async Task<ActionResult<InvitationCreateResponse>> CreateByEmail([FromBody] InvitationCreateByEmailRequest dto)
        {

            Console.WriteLine($"Received invitation request:");
            Console.WriteLine($"TeamId: {dto.TeamId}");
            Console.WriteLine($"InvitedUserEmail: {dto.InvitedUserEmail}");
            Console.WriteLine($"InvitedById: {dto.InvitedById}");
            
            var result = await _invitationService.CreateByEmailAsync(dto);
            Console.WriteLine($"Service result - Success: {result.Success}, IsAlreadyInvited: {result.IsAlreadyInvited}");
            
            if (!result.Success) 
            {
                Console.WriteLine($"Returning BadRequest with error: {result.ErrorMessage}");
                return BadRequest(result);
            }
            

            if (result.IsAlreadyInvited)
            {
                Console.WriteLine($"Returning Ok (200) for already invited case");
                return Ok(result);
            }
            
            Console.WriteLine($"Returning CreatedAtAction (201) for new invitation");
            return CreatedAtAction(nameof(GetById), new { invitationId = result.Id }, result);
        }

        [HttpPost("respond")]
        public async Task<ActionResult<InvitationRespondResponse>> Respond([FromBody] InvitationRespondRequest dto)
        {

            Console.WriteLine($"Received invitation response request:");
            Console.WriteLine($"InvitationId: {dto.InvitationId}");
            Console.WriteLine($"Accept: {dto.Accept}");
            
            var result = await _invitationService.RespondAsync(dto);
            Console.WriteLine($"Service result - Success: {result.Success}, Accepted: {result.Accepted}");
            
            if (!result.Success) 
            {
                Console.WriteLine($"Returning BadRequest with error: {result.ErrorMessage}");
                return BadRequest(result);
            }
            
            Console.WriteLine($"Returning Ok (200) for invitation response");
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