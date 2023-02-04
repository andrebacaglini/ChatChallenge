namespace BotCommandValidator.Interfaces
{
    public interface IStockCommandValidator : ICommandValidator
    {
        bool MessageHasStockCommands(string messageText);
    }
}
