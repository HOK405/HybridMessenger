using MediatR;

namespace HybridMessenger.Application.User.Commands
{
    public class DeleteUserCommand : IRequest
    {
        public int UserIdToDelete { get; set; }
    }
}
