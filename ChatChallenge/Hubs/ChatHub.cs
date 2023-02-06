using BotCommandValidator.Interfaces;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace ChatWebApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IBus _bus;

        public ChatHub(IBus bus)
        {
            _bus = bus;
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

            // Send the same message to the bot queue
            await _bus.Publish(new BotMessage
            {
                UserName = userName,
                MessageText = message,
                MessageDateTime = DateTime.Now
            });

        }
    }
}
