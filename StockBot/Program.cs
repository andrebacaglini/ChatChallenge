using Company.Consumers;
using MassTransit;
using Microsoft.Extensions.Hosting;
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
                    services.AddMassTransit(x =>
                    {
                        //example kebab-case
                        x.SetKebabCaseEndpointNameFormatter();

                        //This looks for consumers under the namespace over the StockBotConsumer class.
                        x.AddConsumersFromNamespaceContaining<StockBotConsumer>();

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host("localhost", "/", x =>
                            {
                                x.Username("admin");
                                x.Password("admin");
                            });

                            cfg.ConfigureEndpoints(context);
                        });
                    });
                });
    }
}
