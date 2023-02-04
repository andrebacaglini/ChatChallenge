using System.Security.Principal;

namespace Contracts
{
    public record ChatMessage
    {
        public string UserName { get; set; } = string.Empty;
        public string ChatMessageText { get; init; } = string.Empty;
        public DateTime ChatMessageDateTime { get; init; }
    }
}