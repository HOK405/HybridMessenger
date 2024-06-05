using MediatR;

namespace HybridMessenger.Application.User.Commands
{
    public class VerifyByEmailPasswordCommand : IRequest<(string, string)>
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
