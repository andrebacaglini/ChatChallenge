using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace HelloWorldBot
{
    public class Consumer : AsyncEventingBasicConsumer, IConsumer
    {
        private readonly IModel _model;
        public Consumer(IModel model) : base(model)
        {
            _model = model;
        }
        public async Task ExecuteAsync()
        {
            _model.ExchangeDeclare("chat", ExchangeType.Topic, false, true);
            var queueName = _model.QueueDeclare("messages", false, false, true).QueueName;

            _model.QueueBind(queueName, "chat", "request");

            var asyncConsumer = new AsyncEventingBasicConsumer(_model);

            _model.BasicConsume(queueName, true, asyncConsumer);

            asyncConsumer.Received += async (obj, deliveryArgs) =>
            {
                var message = Encoding.UTF8.GetString(deliveryArgs.Body.ToArray());
                Console.WriteLine("GetMessage: " + message);

                _model.BasicPublish("chat", "response", null, Encoding.UTF8.GetBytes("HelloWorld!"));
                await Task.Yield();
            };
            await Task.CompletedTask;
        }
    }
}
