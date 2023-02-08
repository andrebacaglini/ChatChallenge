namespace ChatChallenge.StockBot.Consumers
{
    using MassTransit;
    using System;

    public class StockBotConsumerDefinition :
        ConsumerDefinition<StockBotConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<StockBotConsumer> consumerConfigurator)
        {
            if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator e)
            {   
                e.AutoDelete = true;
                e.QueueExpiration = TimeSpan.FromSeconds(30);
            }
        }
    }
}