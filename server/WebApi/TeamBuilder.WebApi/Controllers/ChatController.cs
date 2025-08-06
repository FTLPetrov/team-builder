using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TeamBuilder.Services.Core.Contracts.Chat.Requests;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.WebApi.Hubs;

namespace TeamBuilder.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IChatService chatService, IHubContext<ChatHub> hubContext)
        {
            _chatService = chatService;
            _hubContext = hubContext;
        }

        [HttpGet("team/{teamId}")]
        public async Task<IActionResult> GetTeamMessages(Guid teamId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var messages = await _chatService.GetTeamMessagesAsync(teamId, page, pageSize);
                return Ok(messages);
            }
            catch (Exception)
            {
                return StatusCode(500, new { errorMessage = "Failed to retrieve chat messages" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] ChatCreateRequest request)
        {
            try
            {
                if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is not Guid userId)
                {
                    return Unauthorized();
                }

                var message = await _chatService.CreateMessageAsync(request, userId);
                

                Console.WriteLine($"ChatController: Broadcasting message to team {request.TeamId}");
                await _hubContext.Clients.Group($"team_{request.TeamId}").SendAsync("ReceiveMessage", message);
                Console.WriteLine($"ChatController: Message broadcasted successfully");
                
                return CreatedAtAction(nameof(GetTeamMessages), new { teamId = request.TeamId }, message);
            }
            catch (Exception)
            {
                return StatusCode(500, new { errorMessage = "Failed to create chat message" });
            }
        }
    }
} 