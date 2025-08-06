using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using TeamBuilder.Services.Core.Contracts.Chat.Responses;

namespace TeamBuilder.WebApi.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task JoinTeam(Guid teamId)
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"SignalR: User {userId} joining team {teamId} with connection {Context.ConnectionId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"team_{teamId}");
        }

        public async Task LeaveTeam(Guid teamId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"team_{teamId}");
        }

        public async Task SendMessageToTeam(Guid teamId, ChatResponse message)
        {
            Console.WriteLine($"SignalR: Broadcasting message to team {teamId}");
            await Clients.Group($"team_{teamId}").SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"SignalR: User {userId} connected with connection {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"SignalR: User {userId} disconnected with connection {Context.ConnectionId}");
            if (exception != null)
            {
                Console.WriteLine($"SignalR: Disconnection error: {exception.Message}");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
} 