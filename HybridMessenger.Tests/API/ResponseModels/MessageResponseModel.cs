namespace HybridMessenger.Tests.API.ResponseModels
{
    public class MessageResponseModel
    {
        public int MessageId { get; set; }

        public int ChatId { get; set; }

        public int UserId { get; set; }

        public string MessageText { get; set; }

        public DateTime SentAt { get; set; }

        public string SenderUserName { get; set; }
    }
}
