using MediatR;

namespace HybridMessenger.Application.User.Queries
{
    public class GetUserByIdQuery : IRequest<Domain.Entities.User>
    {
        public Guid Id { get; set; }
    }
}
