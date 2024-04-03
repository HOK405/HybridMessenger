using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HybridMessenger.Domain.Entities
{
    public class ChatMember
    {
        public Guid ChatId { get; set; }

        public Guid UserId { get; set; }

        [Required]
        public DateTime JoinedAt { get; set; }

        public virtual Chat Chat { get; set; }
        public virtual User User { get; set; }
    }

}
