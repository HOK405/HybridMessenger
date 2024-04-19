﻿namespace HybridMessenger.Presentation.RequestModels
{
    public class ChatMessagesRequest
    {
        public string ChatId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SearchValue { get; set; }
        public bool Ascending { get; set; }
        public List<string> Fields { get; set; }
    }
}