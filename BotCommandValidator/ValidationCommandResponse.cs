using BotCommandValidator.Interfaces;

namespace BotCommandValidator
{
    public class ValidationCommandResponse : IValidationCommandResponse
    {
        public ValidationCommandResponse(bool isValid)
        {
            IsValid = isValid;
        }

        public bool IsValid { get; set; }
        public ICollection<string> Commands { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string OriginalMessage { get; set; } = string.Empty;
        public Exception Exception { get; set; }
    }
}