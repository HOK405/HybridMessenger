using HybridMessenger.Application.User.DTOs;
using MediatR;

namespace HybridMessenger.Application.User.Queries
{
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public int Id { get; set; }
    }
}
