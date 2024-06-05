namespace HybridMessenger.Tests.API.ResponseModels
{
    public class ChatResponseModel
    {
        public int ChatId { get; set; }

        public string? ChatName { get; set; }

        public bool IsGroup { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
