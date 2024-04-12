﻿using MediatR;
using Newtonsoft.Json;
using System.ComponentModel;

namespace HybridMessenger.Application.Message.Queries
{
    public class GetPagedUserMessagesQuery : IRequest<IEnumerable<object>>
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
        [DefaultValue("SentAt")]
        public string SortBy { get; set; }
        [DefaultValue("")]
        public string SearchValue { get; set; }
        public bool Ascending { get; set; }
        [DefaultValue(new string[] { })]
        public string[] Fields { get; set; }

        [JsonIgnore]
        public string? UserId { get; set; }
    }
}
