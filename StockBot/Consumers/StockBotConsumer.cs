namespace StockBot.Consumers
{
    using BotCommandValidator.Interfaces;
    using Contracts;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using StockBot.Util;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Bot Consumer.
    /// It listen to his own queue and respond on chat queue.
    /// PS: MassTransit manage the queues by the type of the object.
    /// So BotMessage comes to stock-bot queue and ChatMessage goes to broadcast-message queue.
    /// </summary>
    public class StockBotConsumer : IConsumer<BotMessage>
    {
        private const string OOPS_MESSAGE = "Oops! Something goes wrong... Sorry.";
        private const string BOT_USERNAME = "StockBot";
        private const string NO_STOCK_COMMAND_MESSAGE = "Looks like your message doesn't have a stock command.";
        private const string GETTING_INFO_MESSAGE = "Hey! Let me get this info for you! Please wait a sec..";
        private readonly ILogger<StockBotConsumer> _logger;
        private readonly IBus _bus;
        private readonly IStockCommandValidator _stockCommandValidator;

        public StockBotConsumer(ILogger<StockBotConsumer> logger, IBus bus, IStockCommandValidator stockCommandValidator)
        {
            _logger = logger;
            _bus = bus;
            _stockCommandValidator = stockCommandValidator;
        }

        public async Task Consume(ConsumeContext<BotMessage> context)
        {
            try
            {
                if (_stockCommandValidator.MessageHasStockCommands(context.Message.MessageText))
                {

                    await SendMessageAsync(GETTING_INFO_MESSAGE);

                    var validationResponse = _stockCommandValidator.ValidateCommand(context.Message.MessageText);
                    if (validationResponse.IsValid)
                    {
                        foreach (var command in validationResponse.Commands)
                        {
                            var stockInfoMessage = await GetStocksInfoAsync(command);
                            await SendMessageAsync(stockInfoMessage);
                        }
                    }
                }
                else
                {
                    _logger.LogInformation(NO_STOCK_COMMAND_MESSAGE);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured when trying to get stock information.");
                await SendMessageAsync(OOPS_MESSAGE);
            }
        }

        private static async Task<string> GetStocksInfoAsync(string command)
        {
            using HttpClient client = new();
            var botMessage = string.Empty;

            var stockCode = command.Split('=')[1];

            var response = await client.GetAsync($"https://stooq.com/q/l/?s={stockCode}&f=sd2t2ohlcv&h&e=csv");
            var csvStream = await response.Content.ReadAsStreamAsync();

            var stock = StockDataReader.ReadStockDataFromCSV(csvStream);

            if (stock is null)
            {
                botMessage = $"Looks like I can't retrieve the stock information for the stock code: {stockCode}. Did you wrote the stock code correctly?";
            }
            else
            {
                botMessage = $"{stock.Symbol} quote is ${stock.Close} per share.";
            }

            return botMessage;
        }

        private async Task SendMessageAsync(string message)
        {
            await _bus.Publish(new ChatMessage { UserName = BOT_USERNAME, MessageText = message, MessageDateTime = DateTime.Now });
        }
    }
}