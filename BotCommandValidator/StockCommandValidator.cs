using BotCommandValidator.Interfaces;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace BotCommandValidator
{
    public class StockCommandValidator : IStockCommandValidator
    {   
        private const string STOCK_PATTERN_REGEX = @"(\/stock=[a-zA-Z]{1,5}.[a-zA-Z]{2})";

        public bool MessageHasStockCommands(string message)
        {
            return Regex.Matches(message, STOCK_PATTERN_REGEX).Any();
        }

        public IValidationCommandResponse ValidateCommand(string message)
        {
            var validationResponse = CheckValidations(message);

            return validationResponse;
        }

        private IValidationCommandResponse CheckValidations(string message)
        {
            return CheckCommandsInMessage(message);
        }

        private IValidationCommandResponse CheckCommandsInMessage(string message)
        {
            if (Regex.Matches(message, STOCK_PATTERN_REGEX).Any())
            {
                return ExtractStockCommands(message);
            }
            else
            {
                return new ValidationCommandResponse(false) { ErrorMessage = "The message does not contains a /stock command." };
            }
        }

        public IValidationCommandResponse ExtractStockCommands(string message)
        {
            try
            {
                var distinctedCommands = Regex.Matches(message, STOCK_PATTERN_REGEX).Select(x => x.Value).ToHashSet();
                return new ValidationCommandResponse(true) { Commands = distinctedCommands, OriginalMessage = message };
            }
            catch (ArgumentException e)
            {

                return new ValidationCommandResponse(false)
                {
                    ErrorMessage = "It was not possible to extract the commands from message. Please check the bot logs.",
                    Exception = e
                };
            }


        }

        
    }
}