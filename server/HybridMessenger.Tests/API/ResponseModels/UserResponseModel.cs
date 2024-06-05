namespace HybridMessenger.Tests.API.ResponseModels
{
    public class UserResponseModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
