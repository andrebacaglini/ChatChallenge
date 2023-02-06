using System.Text.RegularExpressions;

namespace ChatWebApp.Util
{
    public static class CommandIdentifier
    {
        private const string STOCK_PATTERN_REGEX = @"(\/stock=[a-zA-Z]{1,5}.[a-zA-Z]{2})";

        public static bool MessageHasStockCommands(string message)
        {
            return Regex.Matches(message, STOCK_PATTERN_REGEX).Any();
        }
    }
}
