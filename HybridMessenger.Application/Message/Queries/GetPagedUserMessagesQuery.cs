using MediatR;

namespace HybridMessenger.Application.Message.Queries
{
    public class GetPagedUserMessagesQuery : IRequest<IEnumerable<object>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SearchValue { get; set; }
        public bool Ascending { get; set; }
        public string[] FieldsToReturn { get; set; }

    }
}
