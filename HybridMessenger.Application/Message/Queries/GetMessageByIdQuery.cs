using MediatR;

namespace HybridMessenger.Application.Message.Queries
{
    public class GetMessageByIdQuery: IRequest<Domain.Entities.Message>
    {
        public int Id { get; set; }
    }
}
