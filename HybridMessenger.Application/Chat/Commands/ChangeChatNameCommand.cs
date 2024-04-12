using HybridMessenger.Application.Chat.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace HybridMessenger.Application.Chat.Commands
{
    public class ChangeChatNameCommand : IRequest<ChatDto>
    {
        public string NewChatName { get; set; }

        public string ChatId { get; set; }

        [JsonIgnore]
        public string UserId { get; set; }
    }
}
