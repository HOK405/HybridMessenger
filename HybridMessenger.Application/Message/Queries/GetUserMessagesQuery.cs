using MediatR;

namespace HybridMessenger.Application.Message.Queries
{
    public class GetUserMessagesQuery : IRequest<IEnumerable<object>>
    {

    }
}
