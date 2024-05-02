using HybridMessenger.Application.Message.Queries;
using HybridMessenger.Tests.API.ResponseModels;

namespace HybridMessenger.Tests.API.Settings
{
    public static class DefaultMessageData
    {
        public static List<MessageResponseModel> GetMessages()
        {
            return new List<MessageResponseModel>
            {
                new MessageResponseModel
                {
                    MessageId = 1,
                    ChatId = 355,
                    UserId = 31,
                    MessageText = "Hello world",
                    SenderUserName = "testUser999"
                },
                new MessageResponseModel
                {
                    MessageId = 3,
                    ChatId = 355,
                    UserId = 31,
                    MessageText = "Test message 2",
                    SenderUserName = "testUser999"
                },
                new MessageResponseModel
                {
                    MessageId = 4,
                    ChatId = 543,
                    UserId = 31,
                    MessageText = "Test message 3",
                    SenderUserName = "testUser999"
                }
            };
        }

        public static GetPagedChatMessagesQuery GetPagedChatMessagesQuery(
            int chatId = 355, 
            int pageNumber = 1, 
            int pageSize = 10, 
            string sortBy = "SentAt", 
            string searchValue = "", 
            bool ascending = true)
        {
            return new GetPagedChatMessagesQuery
            {
                ChatId = chatId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SearchValue = searchValue,
                Ascending = ascending
            };
        }

        public static GetPagedUserMessagesQuery GetPagedUserMessagesQuery(
            int pageNumber = 1,
            int pageSize = 10,
            string sortby = "SentAt",
            string searchValue = "",
            bool ascending = true)
        {
            return new GetPagedUserMessagesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortby,
                SearchValue = searchValue,
                Ascending = ascending
            };
        }
    }
}
