﻿using ChatChallenge.WebApp.Data;
using ChatChallenge.WebApp.Util;
using ChatChallenge.Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatChallenge.WebApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IBus _bus;
        private readonly ApplicationDbContext _context;

        public ChatHub(IBus bus, ApplicationDbContext context)
        {
            _bus = bus;
            _context = context;
        }

        public async Task Send(string userName, string message)
        {
            var chatMessage = new Contracts.ChatMessage
            {
                UserName = userName,
                MessageText = message,
                MessageDateTime = DateTime.Now
            };

            List<Task> tasks = new();

            if (!CommandIdentifier.MessageHasStockCommands(message))
            {
                await _context.ChatMessages.AddAsync(new Data.Model.ChatMessage
                {
                    UserName = chatMessage.UserName,
                    MessageText = chatMessage.MessageText,
                    MessageDateTime = chatMessage.MessageDateTime
                });

                tasks.Add(_context.SaveChangesAsync());
            }
            else
            {
                tasks.Add(_bus.Publish(new BotMessage
                {
                    UserName = userName,
                    MessageText = message,
                    MessageDateTime = DateTime.Now
                }));
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