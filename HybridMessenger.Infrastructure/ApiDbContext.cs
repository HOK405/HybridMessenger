using HybridMessenger.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HybridMessenger.Infrastructure
{
    public class ApiDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public  ApiDbContext(DbContextOptions<ApiDbContext> options) : base (options)
        {
        }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMember> ChatMembers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChatMember>()
                .HasKey(cm => new { cm.ChatId, cm.UserId });

            // Chat and ChatMember relationship
            modelBuilder.Entity<ChatMember>()
                .HasOne(cm => cm.Chat)
                .WithMany(c => c.ChatMembers)
                .HasForeignKey(cm => cm.ChatId);

            modelBuilder.Entity<ChatMember>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.ChatMembers)
                .HasForeignKey(cm => cm.UserId);

            // Contact relationships
            modelBuilder.Entity<Contact>()
                .HasKey(c => new { c.UserId, c.ContactUserId }); // composite key

            modelBuilder.Entity<Contact>()
                .HasOne(c => c.User)
                .WithMany(u => u.Contacts) 
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Contact
            modelBuilder.Entity<Contact>()
                .HasOne(c => c.ContactUser)
                .WithMany() 
                .HasForeignKey(c => c.ContactUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Message relationships
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatID);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.UserID);

            // Ensure MessageID is unique and set as the primary key
            modelBuilder.Entity<Message>()
                .HasKey(m => m.MessageID);

            // Configure Chat entity
            modelBuilder.Entity<Chat>()
                .HasKey(c => c.ChatID); 
        }
    }
}
