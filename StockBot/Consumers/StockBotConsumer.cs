namespace Company.Consumers
{
    using BotCommandValidator.Interfaces;
    using Contracts;
    using MassTransit;
    using MassTransit.Logging;
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
            string botMessage = OOPS_MESSAGE;
            try
            {
                if (_stockCommandValidator.MessageHasStockCommands(context.Message.MessageText))
                {
                    await _bus.Publish(new ChatMessage { UserName = "StockBot", MessageText = $"Hey! Let me get this info for you! Please wait a sec..", MessageDateTime = DateTime.Now });
                    var validationResponse = _stockCommandValidator.ValidateCommand(context.Message.MessageText);
                    if (validationResponse.IsValid)
                    {
                        using HttpClient client = new();
                        foreach (var command in validationResponse.Commands)
                        {
                            var stockCode = command.Split('=')[1];
                            var response = await client.GetAsync($"https://stooq.com/q/l/?s={stockCode}&f=sd2t2ohlcv&h&e=csv");
                            var csvStream = await response.Content.ReadAsStreamAsync();
                            var stock = StockDataReader.ReadStockDataFromCSV(csvStream);
                            
                            if(stock is null)
                            {
                                botMessage = $"Looks like I can't retrieve the stock information for the stock code: {stockCode}. Did you wrote the stock code correctly?";
                            }
                            else
                            {
                                botMessage = $"{stock.Symbol} quote is ${stock.Close} per share.";
                            }
                            await _bus.Publish(new ChatMessage { UserName = "StockBot", MessageText = botMessage, MessageDateTime = DateTime.Now });
                        }
                    }
                }
                else
                {
                    await _bus.Publish(new ChatMessage { UserName = "StockBot", MessageText = $"Looks like your message doesn't have a stock command.", MessageDateTime = DateTime.Now });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,"Unexpected error");
                await _bus.Publish(new ChatMessage { UserName = "StockBot", MessageText = botMessage, MessageDateTime = DateTime.Now });
            }            
        }
    }
}