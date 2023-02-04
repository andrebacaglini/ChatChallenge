using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public record BotMessage
    {
        public string UserName { get; set; } = string.Empty;
        public string ChatMessageText { get; init; } = string.Empty;
        public DateTime ChatMessageDateTime { get; init; }
    }
}
