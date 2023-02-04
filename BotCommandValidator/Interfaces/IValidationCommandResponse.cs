namespace BotCommandValidator.Interfaces
{
    public interface IValidationCommandResponse
    {
        bool IsValid { get; set; }
        ICollection<string> Commands { get; set; }
        string ErrorMessage { get; set; }
        string OriginalMessage { get; set; }
    }
}