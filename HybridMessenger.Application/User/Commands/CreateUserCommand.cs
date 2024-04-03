using MediatR;

namespace HybridMessenger.Application.User.Commands
{
    public class CreateUserCommand : IRequest<Domain.Entities.User>
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
