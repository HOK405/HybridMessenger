using MediatR;
using System.Text.Json.Serialization;

namespace HybridMessenger.Application.Message.Commands
{
    public class DeleteMessageCommand : IRequest
    {
        public int MessageId { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }
    }
}
