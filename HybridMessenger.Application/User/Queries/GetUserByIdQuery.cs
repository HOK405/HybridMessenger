using HybridMessenger.Application.User.DTOs;
using MediatR;

namespace HybridMessenger.Application.User.Queries
{
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public string Id { get; set; }
    }
}
