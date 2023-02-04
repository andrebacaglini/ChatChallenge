namespace Company.Consumers
{
    using Contracts;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The Consumer
    /// </summary>
    public class StockBotConsumer : IConsumer<BotMessage>
    {
        private readonly ILogger<StockBotConsumer> _logger;
        private readonly IBus _bus;

        public StockBotConsumer(ILogger<StockBotConsumer> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public async Task Consume(ConsumeContext<BotMessage> context)
        {
            if (context.Message.ChatMessageText.ToLowerInvariant().Contains("bot"))
            {
                await _bus.Publish(
                    new ChatMessage {UserName="StockBot", ChatMessageText = "Hey! Who called me?", ChatMessageDateTime = DateTime.UtcNow });
            }
        }
    }
}