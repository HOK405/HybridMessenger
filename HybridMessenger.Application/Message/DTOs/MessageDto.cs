namespace HybridMessenger.Application.Message.DTOs
{
    public class MessageDto
    {
        public int MessageId { get; set; }

        public Guid ChatId { get; set; }

        public Guid UserId { get; set; }

        public string MessageText { get; set; }

        public DateTime SentAt { get; set; }
    }
}
