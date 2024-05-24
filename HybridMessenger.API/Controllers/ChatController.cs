using HybridMessenger.Application.Chat.Commands;
using HybridMessenger.Application.Chat.Queries;
using HybridMessenger.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HybridMessenger.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IUserClaimsService _userClaimsService;

        public ChatController(IMediator mediator, IUserClaimsService userClaimsService)
        {
            _mediator = mediator;
            _userClaimsService = userClaimsService;
        }

        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupCommand command)
        {
            command.UserId = _userClaimsService.GetUserId(User);

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("create-private-chat")]
        public async Task<IActionResult> CreatePrivateChat([FromBody] CreatePrivateChatCommand command)
        {
            command.UserCreatorId = _userClaimsService.GetUserId(User);

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("get-my-chats")]
        public async Task<IActionResult> GetUserChats([FromBody] GetPagedUserChatsQuery query)
        {
            query.UserId = _userClaimsService.GetUserId(User);

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("change-chat-name")]
        public async Task<IActionResult> ChangeChatName([FromBody] ChangeGroupNameCommand command)
        {
            command.UserId = _userClaimsService.GetUserId(User);

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPut("add-group-member")]
        public async Task<IActionResult> AddGroupMemberByUsername([FromBody] AddGroupMemberCommand command)
        {
            command.UserId = _userClaimsService.GetUserId(User);

            await _mediator.Send(command);

            return Ok(new { Message = "Group member is successfully added." });
        }

        [HttpPut("remove-group-member")]
        public async Task<IActionResult> RemoveGroupMemberByUsername([FromBody] RemoveGroupMemberCommand command)
        {
            command.UserId = _userClaimsService.GetUserId(User);

            await _mediator.Send(command);

            return Ok(new { Message = "Group member is successfully removed." });
        }

        [HttpPost("delete-chat")]
        public async Task<IActionResult> DeleteChat([FromBody] DeleteChatCommand command)
        {
            command.UserId = _userClaimsService.GetUserId(User);

            await _mediator.Send(command);
            return Ok(new { Message = "Chat is successfully deleted." });
        }
    }
}
