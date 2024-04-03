using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HybridMessenger.Domain.Entities
{
    public class Contact
    {
        public Guid UserId { get; set; }
        public Guid ContactUserId { get; set; }

        [Required]
        public DateTime AddedAt { get; set; }


        public virtual User User { get; set; }
        public virtual User ContactUser { get; set; }
    }

}