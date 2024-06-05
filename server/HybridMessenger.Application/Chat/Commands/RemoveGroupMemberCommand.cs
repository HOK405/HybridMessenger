using MediatR;
using System.Text.Json.Serialization;

namespace HybridMessenger.Application.Chat.Commands
{
    public class RemoveGroupMemberCommand : IRequest
    {
        public int UserIdToRemove { get; set; }
        public int ChatId { get; set; }

        [JsonIgnore]
        public int UserId { get; set; } 
    }
}
