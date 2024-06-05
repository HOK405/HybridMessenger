using HybridMessenger.Application.Chat.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace HybridMessenger.Application.Chat.Commands
{
    public class ChangeGroupNameCommand : IRequest<ChatDto>
    {
        public string NewChatName { get; set; }

        public int ChatId { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }
    }
}
