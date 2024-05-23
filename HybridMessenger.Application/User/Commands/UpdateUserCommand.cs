using HybridMessenger.Application.User.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace HybridMessenger.Application.User.Commands
{
    public class UpdateUserCommand : IRequest<UserDto>
    {
        public string NewUsername { get; set; }
        public string NewPhoneNumber { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
    }
}