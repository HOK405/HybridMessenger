using HybridMessenger.Application.Message.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace HybridMessenger.Application.Message.Commands
{
    public class SendMessageCommand : IRequest<MessageDto>
    {
        public string MessageText { get; set; }

        public int ChatId { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }
    }
}
