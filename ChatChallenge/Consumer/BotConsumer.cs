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
        private readonly IConnectionFactory _connectionFactory;

        public BotConsumer(IHubContext<ChatHub> hubContext, IConnectionFactory connectionFactory)
        {
            _hubContext = hubContext;
            _connectionFactory = connectionFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare("chat", ExchangeType.Direct, true, true);
            channel.QueueDeclare("bot-messages", true, false, true);
            channel.QueueBind("bot-messages", "chat", "response");

            var asyncConsumer = new AsyncEventingBasicConsumer(channel);

            channel.BasicConsume("bot-messages", true, asyncConsumer);

            asyncConsumer.Received += async (obj, deliveryArgs) =>
            {
                var botMessage = JsonConvert.DeserializeObject<ChatMessage>(Encoding.UTF8.GetString(deliveryArgs.Body.ToArray()));
                await _hubContext.Clients.All.SendAsync("broadcastMessage", botMessage, stoppingToken);
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running...");
                await Task.Delay(5000, stoppingToken);
            }            
        }
    }
}
