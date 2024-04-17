using HybridMessenger.Application.Message.Commands;
using HybridMessenger.Application.Message.Queries;
using HybridMessenger.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HybridMessenger.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IUserClaimsService _userClaimsService;

        public MessageController(IMediator mediator, IUserClaimsService userClaimsService)
        {
            _mediator = mediator;
            _userClaimsService = userClaimsService;
        }

        [Authorize]
        [HttpPost("send-message")]
        public async Task<ActionResult> SendMessage([FromBody] SendMessageCommand command)
        {
            command.UserId = _userClaimsService.GetUserId(User);

            await _mediator.Send(command);
            return Ok(new { Message = "Message is sent successfully."});
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
