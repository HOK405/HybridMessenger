using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HybridMessenger.Domain.Entities
{
    public class Message
    {
        public int MessageId { get; set; }

        [ForeignKey("Chat")]
        public int ChatId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        public string MessageText { get; set; }

        [Required]
        public DateTime SentAt { get; set; }


        public virtual Chat Chat { get; set; }
        public virtual User User { get; set; }
    }
}
