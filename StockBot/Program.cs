using BotCommandValidator;
using BotCommandValidator.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using StockBot.Services;
using System.Threading.Tasks;

namespace StockBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IConnectionFactory, ConnectionFactory>((provider) =>
                    {
                        var connectionFactory = new ConnectionFactory
                        {
                            HostName = "172.17.0.2",
                            VirtualHost = "/",
                            UserName = "admin",
                            Password = "admin",
                            DispatchConsumersAsync = true
                        };
                        return connectionFactory;
                    });

                    services.AddTransient<IStockCommandValidator, StockCommandValidator>();

                    services.AddHostedService<ChatRequestMessagesService>();
                });
    }
}
