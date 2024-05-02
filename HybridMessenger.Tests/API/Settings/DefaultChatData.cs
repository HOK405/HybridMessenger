using HybridMessenger.Application.Chat.Queries;

namespace HybridMessenger.Tests.API.Settings
{
    public static class DefaultChatData
    {
        public static GetPagedUserChatsQuery GetPagedUserChatsQuery(
            int pageNumber = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            string searchValue = "",
            bool ascending = false,
            string[] fields = null) 
        {
            fields = fields ?? Array.Empty<string>();

            return new GetPagedUserChatsQuery
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
