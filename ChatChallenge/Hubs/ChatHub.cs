using ChatWebApp.Data;
using ChatWebApp.Data.Model;
using ChatWebApp.Util;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace ChatWebApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly IConnectionFactory _connectionFactory;

        public ChatHub(ApplicationDbContext context, IConnectionFactory connectionFactory)
        {
            _context = context;
            _connectionFactory = connectionFactory;
        }

        public async Task Send(string userName, string message)
        {
            var chatMessage = new ChatMessage
            {
                UserName = userName,
                MessageText = message,
                MessageDateTime = DateTime.Now
            };

            List<Task> tasks = new();

            if (!CommandIdentifier.MessageHasStockCommands(message))
            {
                await _context.ChatMessages.AddAsync(new ChatMessage
                {
                    UserName = chatMessage.UserName,
                    MessageText = chatMessage.MessageText,
                    MessageDateTime = chatMessage.MessageDateTime
                });

                tasks.Add(_context.SaveChangesAsync());
            }
            else
            {
                using var connection = _connectionFactory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.ExchangeDeclare("chat", ExchangeType.Direct, true, true);
                channel.BasicPublish("chat", "request", null, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(chatMessage)));
            }

            // Send the message from client to signalR
            tasks.Add(Clients.All.SendAsync("broadcastMessage", chatMessage));

            Task.WaitAll(tasks.ToArray());
        }

        public async Task RetriveChatHistory()
        {
            var history = _context.ChatMessages.OrderByDescending(x => x.MessageDateTime).AsNoTracking().Take(50).ToList();
            await Clients.Caller.SendAsync("loadChatHistory", history.Reverse<Data.Model.ChatMessage>());
        }
    }
}