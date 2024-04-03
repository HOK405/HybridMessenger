using MediatR;

namespace HybridMessenger.Application.User.Commands
{
    public class RegisterUserCommand : IRequest<string>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
