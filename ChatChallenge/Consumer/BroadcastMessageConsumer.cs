using ChatWebApp.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace ChatWebApp.Consumer
{
    public class BroadcastMessageConsumer : IConsumer<BroadcastMessage>
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public BroadcastMessageConsumer(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<BroadcastMessage> context)
        {
            await _hubContext.Clients.All.SendAsync("broadcastMessage", context.Message.Name, context.Message.Message);
        }
    }

    public record BroadcastMessage(string Name, string Message);

}
