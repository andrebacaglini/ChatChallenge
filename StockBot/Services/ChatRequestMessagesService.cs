namespace StockBot.Services
{
    using BotCommandValidator.Interfaces;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using StockBot.Model;
    using StockBot.Util;
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class ChatRequestMessagesService : BackgroundService
    {
        private const string OOPS_MESSAGE = "Oops! Something goes wrong... Sorry.";
        private const string BOT_USERNAME = "StockBot";
        private const string NO_STOCK_COMMAND_MESSAGE = "Looks like your message doesn't have a stock command.";
        private const string GETTING_INFO_MESSAGE = "Hey! Let me get this info for you! Please wait a sec..";

        private readonly ILogger<ChatRequestMessagesService> _logger;
        private readonly IModel _model;
        private readonly IStockCommandValidator _stockCommandValidator;

        public ChatRequestMessagesService(ILogger<ChatRequestMessagesService> logger, IModel model, IStockCommandValidator stockCommandValidator)
        {
            _logger = logger;
            _model = model;
            _stockCommandValidator = stockCommandValidator;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var asyncConsumer = new AsyncEventingBasicConsumer(_model);
                _model.BasicConsume("messages", true, asyncConsumer);
                asyncConsumer.Received += ConsumeChatMessages;

                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Worker running...");
                    await Task.Delay(5000, stoppingToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured when trying to get stock information.");
                SendMessageAsync(OOPS_MESSAGE);
            }
        }

        private async Task ConsumeChatMessages(object sender, BasicDeliverEventArgs eventArgs)
        {
            if (eventArgs.RoutingKey == "request")
            {
                var message = JsonConvert.DeserializeObject<ChatMessage>(Encoding.UTF8.GetString(eventArgs.Body.ToArray()));

                if (_stockCommandValidator.MessageHasStockCommands(message.MessageText))
                {
                    await SendMessageAsync(GETTING_INFO_MESSAGE);

                    var validationResponse = _stockCommandValidator.ValidateCommand(message.MessageText);
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
            try
            {
                var botMessage = new ChatMessage { UserName = BOT_USERNAME, MessageText = message, MessageDateTime = DateTime.Now };
                _model.BasicPublish("chat", "response", null, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(botMessage)));
                await Task.Yield();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something goes wrong when SendMessageAsync");
            }
        }
    }
}