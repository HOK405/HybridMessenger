using HybridMessenger.Application.Chat.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace HybridMessenger.Application.Chat.Commands
{
    public class CreateGroupCommand : IRequest<ChatDto>
    {
        public string ChatName { get; set; }
        [JsonIgnore]
        public int UserId;
    }
}
