namespace HybridMessenger.Application.Message.DTOs
{
    public class MessageRequestModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SearchValue { get; set; }
        public bool Ascending { get; set; }
        public string[] Fields { get; set; }
    }
}
