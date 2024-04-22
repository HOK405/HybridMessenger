namespace HybridMessenger.Presentation.RequestModels
{
    public class ChatMessagesRequest
    {
        public int ChatId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public string SortBy { get; set; }
        public string SearchValue { get; set; } = "";
        public bool Ascending { get; set; } = true;
        public List<string> Fields { get; set; } = new List<string>();
    }
}
