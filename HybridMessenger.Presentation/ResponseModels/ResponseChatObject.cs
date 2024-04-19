using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridMessenger.Presentation.ResponseModels
{
    public class ResponeChatObject
    {
        public Guid ChatId { get; set; }

        public string? ChatName { get; set; }

        public bool IsGroup { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
