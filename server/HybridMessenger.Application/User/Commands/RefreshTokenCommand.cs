using MediatR;

namespace HybridMessenger.Application.User.Commands
{
    public class RefreshTokenCommand : IRequest<string>
    {
        public string RefreshToken { get; set; }
    }
}