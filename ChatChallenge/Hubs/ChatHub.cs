using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Identity;
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
                ChatMessageText = message,
                ChatMessageDateTime = DateTime.UtcNow
            };

            // Send the message from client to signalR
            await Clients.All.SendAsync("broadcastMessage", chatMessage);

            // Send another message to a queue created based on the type of message.
            await _bus.Publish(new BotMessage
            {
                UserName = userName,
                ChatMessageText = message,
                ChatMessageDateTime = DateTime.UtcNow

            });
        }
    }
}
