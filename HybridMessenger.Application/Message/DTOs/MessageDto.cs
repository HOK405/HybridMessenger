namespace HybridMessenger.Application.Message.DTOs
{
    public class MessageDto
    {
        public int MessageId { get; set; }

        public int ChatId { get; set; }

        public int UserId { get; set; }

        public string MessageText { get; set; }

        public DateTime SentAt { get; set; }

        public string SenderUserName { get; set; }
    }
}
