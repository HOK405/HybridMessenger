using MediatR;

namespace HybridMessenger.Application.User.Queries
{
    public class GetEmailByUsernameQuery : IRequest<string>
    {
        public string Username { get; set; }
    }
}