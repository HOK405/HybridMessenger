namespace HybridMessenger.Presentation.RequestModels
{
    public class PaginationRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string SortBy { get; set; }
        public string SearchValue { get; set; } = "";
        public bool Ascending { get; set; } = true;
        public List<string> Fields { get; set; } = new List<string>();
    }
}
