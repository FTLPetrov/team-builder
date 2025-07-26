using Microsoft.AspNetCore.Mvc;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Services.Core.Contracts.Chat.Requests;
using TeamBuilder.Services.Core.Contracts.Chat.Responses;

namespace TeamBuilder.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<IEnumerable<ChatResponse>>> GetAll(Guid teamId)
            => Ok(await _chatService.GetAllAsync(teamId));

        [HttpGet("{chatId}")]
        public async Task<ActionResult<ChatResponse>> GetById(Guid chatId)
        {
            var chat = await _chatService.GetByIdAsync(chatId);
            if (chat == null) return NotFound();
            return Ok(chat);
        }

        [HttpPost]
        public async Task<ActionResult<ChatCreateResponse>> Create([FromBody] ChatCreateRequest dto)
        {
            var result = await _chatService.CreateAsync(dto);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { chatId = result.Id }, result);
        }

        [HttpDelete("{chatId}")]
        public async Task<IActionResult> Delete(Guid chatId)
        {
            var deleted = await _chatService.DeleteAsync(chatId);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
} 