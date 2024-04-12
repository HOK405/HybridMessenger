﻿using HybridMessenger.Application.Chat.Commands;
using HybridMessenger.Application.Chat.Queries;
using HybridMessenger.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HybridMessenger.API.Controllers
{
    public class ChatController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IUserClaimsService _userClaimsService;

        public ChatController(IMediator mediator, IUserClaimsService userClaimsService)
        {
            _mediator = mediator;
            _userClaimsService = userClaimsService;
        }

        [Authorize]
        [HttpPost("create-group")]
        public async Task<ActionResult> CreateGroup([FromBody] CreateGroupCommand command)
        {
            command.UserId = _userClaimsService.GetUserId(User);

            if (!string.IsNullOrEmpty(command.UserId))
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
            command.UserCreatorId = _userClaimsService.GetUserId(User);

            if (!string.IsNullOrEmpty(command.UserCreatorId))
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
            query.UserId = _userClaimsService.GetUserId(User);

            if (!string.IsNullOrEmpty(query.UserId))
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
        [HttpPut("change-chat-name")]
        public async Task<ActionResult> ChangeChatName([FromBody] ChangeChatNameCommand command)
        {
            command.UserId = _userClaimsService.GetUserId(User);

            if(!string.IsNullOrEmpty(command.UserId))
            {
                var result = await _mediator.Send(command);

                if (result is not null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
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
            command.UserId = _userClaimsService.GetUserId(User);

            if (!string.IsNullOrEmpty(command.UserId))
            {
                var result = await _mediator.Send(command);
                if (result)
                {
                    return Ok("Chat is successfully deleted.");
                }
                else
                {
                    return BadRequest("The specified chat doesn't exist in the system.");
                }
            }
            else
            {
                return Unauthorized("UserId claim is missing in the token.");
            }
        }
    }
}