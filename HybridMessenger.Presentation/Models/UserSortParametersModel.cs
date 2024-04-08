namespace HybridMessenger.Presentation.Models
{
    public class UserSortParametersModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SearchValue { get; set; }
        public bool Ascending { get; set; }
    }
}
