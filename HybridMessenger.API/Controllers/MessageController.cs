using HybridMessenger.Application.Message.Commands;
using HybridMessenger.Application.Message.Queries;
using HybridMessenger.Domain.Services;
using HybridMessenger.Infrastructure.Hubs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace HybridMessenger.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IUserClaimsService _userClaimsService;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageController(IMediator mediator, IUserClaimsService userClaimsService, IHubContext<ChatHub> hubContext)
        {
            _mediator = mediator;
            _userClaimsService = userClaimsService;
            _hubContext = hubContext;
        }

        [Authorize]
        [HttpPost("get-chat-messages")]
        public async Task<ActionResult> GetChatMessages([FromBody] GetPagedChatMessagesQuery query)
        {
            query.UserId = _userClaimsService.GetUserId(User);

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("get-user-messages")]
        public async Task<ActionResult> GetUserMessages([FromBody] GetPagedUserMessagesQuery query)
        {
            query.UserId = _userClaimsService.GetUserId(User);

            var result = await _mediator.Send(query); 
            return Ok(result);
        }
    }
}
