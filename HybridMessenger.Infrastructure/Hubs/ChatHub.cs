using HybridMessenger.Application.Message.Commands;
using HybridMessenger.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HybridMessenger.Infrastructure.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly IUserClaimsService _userClaimsService;

        public ChatHub(IMediator mediator, IUserClaimsService userClaimsService)
        {
            _mediator = mediator;
            _userClaimsService = userClaimsService;
        }

        public async Task JoinGroup(string groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
        }

        public async Task LeaveGroup(string groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToString());
        }

        public async Task SendMessage(string groupId, string messageText)
        {
            var userId = _userClaimsService.GetUserId(Context.User);
            var command = new SendMessageCommand
            {
                ChatId = groupId.ToString(),
                MessageText = messageText,
                UserId = userId
            };

            var messageDto = await _mediator.Send(command);

            // Send to all clients in the specified group
            await Clients.Group(groupId.ToString()).SendAsync("ReceiveMessage", messageDto);
        }
    }

}
