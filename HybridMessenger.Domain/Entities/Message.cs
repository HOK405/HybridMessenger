using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HybridMessenger.Domain.Entities
{
    public class Message
    {
        public int MessageID { get; set; }

        [ForeignKey("Chat")]
        public Guid ChatID { get; set; }

        [ForeignKey("User")]
        public Guid UserID { get; set; }

        [Required]
        public string MessageText { get; set; }

        [Required]
        public DateTime SentAt { get; set; }


        public virtual Chat Chat { get; set; }
        public virtual User User { get; set; }
    }
}
