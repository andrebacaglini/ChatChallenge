namespace ChatChallenge.BotCommandValidator.Interfaces
{
    public interface IStockCommandValidator : ICommandValidator
    {
        bool MessageHasStockCommands(string messageText);
    }
}
