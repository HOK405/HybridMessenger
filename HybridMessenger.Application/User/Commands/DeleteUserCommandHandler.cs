using HybridMessenger.Domain.Services;
using MediatR;

namespace HybridMessenger.Application.User.Commands
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUserIdentityService _userService;
        public DeleteUserCommandHandler(IUserIdentityService userService)
        {
            _userService = userService;
        }
        public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            await _userService.DeleteUserByIdAsync(command.UserIdToDelete);
        }
    }
}
