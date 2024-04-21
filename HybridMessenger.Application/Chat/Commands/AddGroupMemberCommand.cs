using MediatR;
using System.Text.Json.Serialization;

namespace HybridMessenger.Application.Chat.Commands
{
    public class AddGroupMemberCommand : IRequest
    {
        public string UserNameToAdd { get; set; }

        public int ChatId {  get; set; }

        [JsonIgnore]
        public int UserId { get; set; }
    }
}
