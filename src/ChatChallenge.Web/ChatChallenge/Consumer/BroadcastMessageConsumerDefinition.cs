using MassTransit;

namespace ChatChallenge.WebApp.Consumer
{
    public class BroadcastMessageConsumerDefinition : ConsumerDefinition<BroadcastMessageConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<BroadcastMessageConsumer> consumerConfigurator)
        {
            if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator e)
            {
                e.AutoDelete = true;
                e.QueueExpiration = TimeSpan.FromSeconds(30);
            }
        }
    }
}
