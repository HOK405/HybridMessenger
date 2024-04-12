using HybridMessenger.Application.Chat.Commands;
using HybridMessenger.Application.Chat.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HybridMessenger.API.Controllers
{
    public class ChatController : Controller
    {
        private readonly IMediator _mediator;

        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost("create-group")]
        public async Task<ActionResult> CreateGroup([FromBody] CreateGroupCommand command)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            command.UserId = userId;

            if (!string.IsNullOrEmpty(userId))
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            else
            {
                return Unauthorized("UserId claim is missing in the token.");
            }
        }

        [Authorize]
        [HttpPost("create-private-chat")]
        public async Task<ActionResult> CreatePrivateChat([FromBody] CreatePrivateChatCommand command)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            command.UserCreatorId = userId;

            if (!string.IsNullOrEmpty(userId))
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            else
            {
                return Unauthorized("UserId claim is missing in the token.");
            }
        }

        [Authorize]
        [HttpPost("get-paged-chats")]
        public async Task<ActionResult> GetUserChats([FromBody] GetPagedUserChatsQuery query)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            query.UserId = userId;

            if (!string.IsNullOrEmpty(userId))
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            else
            {
                return Unauthorized("UserId claim is missing in the token.");
            }
        }

        [Authorize]
        [HttpPost("delete-chat")]
        public async Task<ActionResult> DeleteChat([FromBody] DeleteChatCommand command)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            command.UserId = userId;

            if (!string.IsNullOrEmpty(userId))
            {
                var result = await _mediator.Send(command);
                if (result)
                {
                    return Ok("Chat is successfully deleted.");
                }
                else
                {
                    return Ok("The specified chat doesn't exist in the system.");
                }
            }
            else
            {
                return Unauthorized("UserId claim is missing in the token.");
            }
        }
    }
}
