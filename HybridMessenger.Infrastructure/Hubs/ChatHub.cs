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
        private static readonly Dictionary<string, HashSet<string>> ChatConnections = new();

        public ChatHub(IMediator mediator, IUserClaimsService userClaimsService)
        {
            _mediator = mediator;
            _userClaimsService = userClaimsService;
        }

        public async Task StartCall(string chatId)
        {
            if (ChatConnections.ContainsKey(chatId))
            {
                var connections = ChatConnections[chatId];
                foreach (var connectionId in connections)
                {
                    foreach (var otherConnectionId in connections)
                    {
                        if (connectionId != otherConnectionId)
                        {
                            await Clients.Client(connectionId).SendAsync("ConnectPeer", otherConnectionId);
                        }
                    }
                }
            }
            await Clients.OthersInGroup(chatId).SendAsync("CallStarted", chatId);
        }

        public async Task SendOffer(string chatId, string callerId, string offer)
        {
            await Clients.Client(callerId).SendAsync("ReceiveOffer", chatId, Context.ConnectionId, offer);
        }

        public async Task SendAnswer(string chatId, string callerId, string answer)
        {
            await Clients.Client(callerId).SendAsync("ReceiveAnswer", Context.ConnectionId, answer);
        }

        public async Task SendIceCandidate(string chatId, string candidate)
        {
            await Clients.OthersInGroup(chatId).SendAsync("ReceiveIceCandidate", Context.ConnectionId, candidate);
        }

        public async Task<string> JoinChat(string chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);

            if (!ChatConnections.ContainsKey(chatId))
            {
                ChatConnections[chatId] = new HashSet<string>();
            }

            ChatConnections[chatId].Add(Context.ConnectionId);

            await Clients.OthersInGroup(chatId).SendAsync("ConnectPeer", Context.ConnectionId);
            await Clients.OthersInGroup(chatId).SendAsync("Send", $"{Context.ConnectionId} has joined the chat {chatId}.");
            return Context.ConnectionId;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            foreach (var chat in ChatConnections)
            {
                if (chat.Value.Remove(Context.ConnectionId))
                {
                    await Clients.Group(chat.Key).SendAsync("Send", $"{Context.ConnectionId} has left the chat {chat.Key}.");
                }
            }

            await base.OnDisconnectedAsync(exception);
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
