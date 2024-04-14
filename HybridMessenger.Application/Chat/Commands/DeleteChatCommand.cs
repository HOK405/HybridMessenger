﻿using MediatR;
using System.Text.Json.Serialization;

namespace HybridMessenger.Application.Chat.Commands
{
    public class DeleteChatCommand : IRequest
    {
        public string ChatId { get; set; }

        [JsonIgnore]
        public string UserId { get; set; }
    }
}
