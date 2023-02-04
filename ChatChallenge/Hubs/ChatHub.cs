using BotCommandValidator.Interfaces;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace ChatWebApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IBus _bus;
        private readonly IStockCommandValidator _stockCommandValidator;

        public ChatHub(IBus bus, IStockCommandValidator validator)
        {
            _bus = bus;
            _stockCommandValidator = validator;
        }

        public async Task Send(string userName, string message)
        {
            var chatMessage = new ChatMessage
            {
                UserName = userName,
                MessageText = message,
                MessageDateTime = DateTime.Now
            };

            // Send the message from client to signalR
            await Clients.All.SendAsync("broadcastMessage", chatMessage);


            // Check the message to see if bot is requested.
            if (_stockCommandValidator.MessageHasStockCommands(message))
            {
                // Send another message to a queue created based on the type of message that the consumer are expecting.
                await _bus.Publish(new BotMessage
                {
                    UserName = userName,
                    MessageText = message,
                    MessageDateTime = DateTime.Now
                });
            }
        }
    }
}
