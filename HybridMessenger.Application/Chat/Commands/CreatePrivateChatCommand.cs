using HybridMessenger.Application.Chat.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace HybridMessenger.Application.Chat.Commands
{
    public class CreatePrivateChatCommand : IRequest<ChatDto>
    {
        public string UserNameToCreateWith { get; set; }
        [JsonIgnore]
        public int UserCreatorId { get; set; }
    }
}
