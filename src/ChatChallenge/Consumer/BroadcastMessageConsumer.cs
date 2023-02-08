using ChatChallenge.WebApp.Hubs;
using ChatChallenge.Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace ChatChallenge.WebApp.Consumer
{
    public class BroadcastMessageConsumer : IConsumer<ChatMessage>
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public BroadcastMessageConsumer(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// Consumes the queue and send the message to the SignalR chat.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<ChatMessage> context)
        {
            // sends the data consumed from rabbitmq to the SignalR
            await _hubContext.Clients.All.SendAsync("broadcastMessage", context.Message);
        }
    }
}
