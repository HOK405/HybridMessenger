namespace HybridMessenger.Application.Chat.DTOs
{
    public class ChatDto
    {
        public Guid ChatId { get; set; }

        public string? ChatName { get; set; }

        public bool IsGroup { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
