using HelloWorldBot;
using RabbitMQ.Client;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
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

        services.AddSingleton((provider) =>
        {
            var connectionFactory = provider.GetRequiredService<IConnectionFactory>();
            return connectionFactory.CreateConnection().CreateModel();
        });

        services.AddSingleton<IConsumer, Consumer>();

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
