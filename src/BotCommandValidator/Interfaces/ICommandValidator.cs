using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatChallenge.BotCommandValidator.Interfaces
{
    public interface ICommandValidator
    {
        /// <summary>
        /// Should identify if the message has the basic syntax for commands.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        IValidationCommandResponse ValidateCommand(string message);
    }
}
