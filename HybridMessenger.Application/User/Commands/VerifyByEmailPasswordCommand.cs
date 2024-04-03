using MediatR;

namespace HybridMessenger.Application.User.Commands
{
    public class VerifyByEmailPasswordCommand : IRequest<Domain.Entities.User>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
