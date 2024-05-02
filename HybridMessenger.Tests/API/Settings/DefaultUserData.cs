using HybridMessenger.Application.User.Commands;
using HybridMessenger.Application.User.Queries;
using HybridMessenger.Tests.API.ResponseModels;

namespace HybridMessenger.Tests.API.Settings
{
    public static class DefaultUserData
    {
        public static List<UserResponseModel> GetUsers()
        {
            return new List<UserResponseModel>
            {
                new UserResponseModel
                {
                    Id = 31,
                    UserName = "testUser999",
                    Email = "testUser999@mail.com",
                    PhoneNumber = "+91231212121",
                },
                new UserResponseModel
                {
                    Id = 49,
                    UserName = "userToChatWith123",
                    Email = "userToChatWith123@mail.com",
                    PhoneNumber = "+12312311231",
                }
            };
        }

        public static VerifyByEmailPasswordCommand GetLoginCommand(
            string email = "testUser999@mail.com",
            string password = "testUser999")
        {
            return new VerifyByEmailPasswordCommand
            {
                Email = email,
                Password = password
            };
        }

        public static GetPagedUsersQuery GetPagedUsersQuery(
            int pageNumber = 1,
            int pageSize = 2,
            string sortBy = "CreatedAt",
            string searchValue = "",
            bool ascending = true,
            string[] fields = null)
        {
            fields = fields ?? Array.Empty<string>(); 

            return new GetPagedUsersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SearchValue = searchValue,
                Ascending = ascending,
                Fields = fields
            };
        }
    }
}
