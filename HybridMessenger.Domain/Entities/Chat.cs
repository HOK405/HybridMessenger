using System.ComponentModel.DataAnnotations;

namespace HybridMessenger.Domain.Entities
{
    public class Chat
    {
        public Guid ChatID { get; set; }

        [MaxLength(255)]
        public string? ChatName { get; set; }

        [Required]
        public bool IsGroup { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public ICollection<ChatMember> ChatMembers { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
