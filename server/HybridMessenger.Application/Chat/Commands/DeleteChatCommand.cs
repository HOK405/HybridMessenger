using MediatR;
using System.Text.Json.Serialization;

namespace HybridMessenger.Application.Chat.Commands
{
    public class DeleteChatCommand : IRequest
    {
        public int ChatId { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }
    }
}
