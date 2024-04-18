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
        public async Task SendMessage(string chatId, string messageText)
        {
            var userId = _userClaimsService.GetUserId(Context.User);

            var command = new SendMessageCommand
            {
                ChatId = chatId,
                MessageText = messageText,
                UserId = userId
            };

            var messageDto = await _mediator.Send(command);

            await Clients.All.SendAsync("ReceiveMessage", messageDto);
        }
    }
}
