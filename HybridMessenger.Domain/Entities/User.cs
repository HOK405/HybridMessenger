using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HybridMessenger.Domain.Entities
{
    public class User : IdentityUser<int>
    {
        [Required]
        public DateTime CreatedAt { get; set; }


        public ICollection<Contact> Contacts { get; set; }
        public ICollection<ChatMember> ChatMembers { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
