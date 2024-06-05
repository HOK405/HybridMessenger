using MediatR;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace HybridMessenger.Application.Message.Queries
{
    public class GetPagedChatMessagesQuery : IRequest<IEnumerable<object>>
    {
        public int ChatId { get; set; }

        [DefaultValue(1)]
        public int PageNumber { get; set; }

        [DefaultValue(10)]
        public int PageSize { get; set; }

        [DefaultValue("SentAt")]
        public string SortBy { get; set; }

        [DefaultValue("")]
        public string SearchValue { get; set; }

        public bool Ascending { get; set; }

        [DefaultValue(new string[] { })]
        public string[] Fields { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }
    }
}
