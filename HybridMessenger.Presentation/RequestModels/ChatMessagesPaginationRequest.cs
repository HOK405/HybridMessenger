namespace HybridMessenger.Presentation.RequestModels
{
    public class ChatMessagesPaginationRequest : PaginationRequest
    {
        public int ChatId { get; set; }
    }
}
