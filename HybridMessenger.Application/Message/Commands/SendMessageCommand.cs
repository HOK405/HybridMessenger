using HybridMessenger.Application.Message.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace HybridMessenger.Application.Message.Commands
{
    public class SendMessageCommand : IRequest
    {
        public string MessageText { get; set; }

        public string ChatId { get; set; }

        [JsonIgnore]
        public string UserId { get; set; }

    }
}
