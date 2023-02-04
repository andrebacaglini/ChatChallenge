using ChatWebApp.Hubs;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace ChatWebApp.Consumer
{
    public class BroadcastMessageConsumer : IConsumer<ChatMessage>
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public BroadcastMessageConsumer(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<ChatMessage> context)
        {
            // sends the data consumed from rabbitmq to the SignalR
            await _hubContext.Clients.All.SendAsync("broadcastMessage", context.Message);
        }
    }
}
