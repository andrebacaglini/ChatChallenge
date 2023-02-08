using ChatWebApp.Data.Model;
using ChatWebApp.Hubs;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ChatWebApp.Consumer
{
    public class BotConsumer : BackgroundService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IModel _model;

        public BotConsumer(IHubContext<ChatHub> hubContext, IModel model)
        {
            _hubContext = hubContext;
            _model = model;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var asyncConsumer = new AsyncEventingBasicConsumer(_model);

            _model.BasicConsume("messages", true, asyncConsumer);

            asyncConsumer.Received += async (obj, deliveryArgs) =>
            {
                var botMessage = JsonConvert.DeserializeObject<ChatMessage>(Encoding.UTF8.GetString(deliveryArgs.Body.ToArray()));
                await _hubContext.Clients.All.SendAsync("broadcastMessage", botMessage, stoppingToken);
            };
            await Task.CompletedTask;
        }
    }
}
