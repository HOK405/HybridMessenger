using HybridMessenger.Application.Message.Commands;
using HybridMessenger.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HybridMessenger.Infrastructure.Hubs
{
    /*[Authorize]*/
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly IUserClaimsService _userClaimsService;

        public ChatHub(IMediator mediator, IUserClaimsService userClaimsService)
        {
            _mediator = mediator;
            _userClaimsService = userClaimsService;
        }

        public async Task SendOffer(string groupName, string offer)
        {
            await Clients.Group(groupName).SendAsync("ReceiveOffer", Context.ConnectionId, offer);
        }

        public async Task SendAnswer(string groupName, string answer)
        {
            await Clients.Group(groupName).SendAsync("ReceiveAnswer", Context.ConnectionId, answer);
        }

        public async Task SendIceCandidate(string groupName, string candidate)
        {
            await Clients.Group(groupName).SendAsync("ReceiveIceCandidate", Context.ConnectionId, candidate);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }

        public async Task SendMessage(int groupId, string messageText)
        {
            var userId = _userClaimsService.GetUserId(Context.User);

            var command = new SendMessageCommand
            {
                ChatId = groupId,
                MessageText = messageText,
                UserId = userId
            };

            var messageDto = await _mediator.Send(command);

            // Send to all clients in the specified group
            await Clients.Group(groupId.ToString()).SendAsync("ReceiveMessage", messageDto);
        }
    }
}
