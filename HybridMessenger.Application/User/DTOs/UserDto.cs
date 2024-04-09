namespace HybridMessenger.Application.User.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
