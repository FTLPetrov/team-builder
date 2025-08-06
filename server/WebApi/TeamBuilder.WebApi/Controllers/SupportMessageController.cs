using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamBuilder.Data;
using TeamBuilder.Data.Models;
using System.Security.Claims;

namespace TeamBuilder.WebApi.Controllers
{
    [ApiController]
    [Route("api/support-messages")]
    [Authorize]
    public class SupportMessageController : ControllerBase
    {
        private readonly TeamBuilderDbContext _context;

        public SupportMessageController(TeamBuilderDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> CreateSupportMessage([FromBody] CreateSupportMessageRequest request)
        {
            try
            {
                Console.WriteLine($"SupportMessageController: Starting to process support message request");
                Console.WriteLine($"SupportMessageController: Request Subject: {request.Subject}");
                Console.WriteLine($"SupportMessageController: Request Message: {request.Message}");
                
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"SupportMessageController: UserId from claims: {userId}");
                
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    Console.WriteLine($"SupportMessageController: Invalid or missing userId: {userId}");
                    return Unauthorized();
                }

                var user = await _context.Users.FindAsync(userGuid);
                if (user == null)
                {
                    Console.WriteLine($"SupportMessageController: User not found in database for ID: {userGuid}");
                    return Unauthorized();
                }

                Console.WriteLine($"SupportMessageController: User found: {user.FirstName} {user.LastName}");

                var supportMessage = new SupportMessage(request.Subject, request.Message, userGuid);
                Console.WriteLine($"SupportMessageController: Created support message object with ID: {supportMessage.Id}");
                
                _context.SupportMessages.Add(supportMessage);
                Console.WriteLine($"SupportMessageController: Added support message to context");
                
                var result = await _context.SaveChangesAsync();
                Console.WriteLine($"SupportMessageController: SaveChangesAsync result: {result} entities affected");

                Console.WriteLine($"SupportMessageController: Support message saved successfully with ID: {supportMessage.Id}");
                return Ok(new { message = "Support message submitted successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SupportMessageController: Exception occurred: {ex.Message}");
                Console.WriteLine($"SupportMessageController: Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Failed to submit support message" });
            }
        }
    }

    public class CreateSupportMessageRequest
    {
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
} 