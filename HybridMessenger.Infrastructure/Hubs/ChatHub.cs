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

        public async Task StartCall(string chatId)
        {
            await Clients.OthersInGroup(chatId).SendAsync("CallStarted", chatId);
        }

        public async Task SendOffer(string chatId, string offer)
        {
            await Clients.Group(chatId).SendAsync("ReceiveOffer",chatId, Context.ConnectionId, offer);
        }

        public async Task SendAnswer(string chatId, string answer)
        {
            await Clients.Group(chatId).SendAsync("ReceiveAnswer", Context.ConnectionId, answer);
        }

        public async Task SendIceCandidate(string chatid, string candidate)
        {
            await Clients.Group(chatid).SendAsync("ReceiveIceCandidate", Context.ConnectionId, candidate);
        }

        public async Task JoinChat(string chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);

            await Clients.Group(chatId).SendAsync("UserJoined", Context.ConnectionId);
            await Clients.Group(chatId).SendAsync("Send", $"{Context.ConnectionId} has joined the chat {chatId}.");
        }

        public async Task LeaveChat(string chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
            await Clients.Group(chatId).SendAsync("Send", $"{Context.ConnectionId} has left the chat {chatId}.");
        }

        public async Task SendMessage(int chatId, string messageText)
        {
            var userId = _userClaimsService.GetUserId(Context.User);

            var command = new SendMessageCommand
            {
                ChatId = chatId,
                MessageText = messageText,
                UserId = userId
            };

            var messageDto = await _mediator.Send(command);

            // Send to all clients in the specified group
            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", messageDto);
        }
    }
}
