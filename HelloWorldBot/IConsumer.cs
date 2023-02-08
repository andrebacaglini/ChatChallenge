using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorldBot
{
    public interface IConsumer : IAsyncBasicConsumer
    {
        Task ExecuteAsync();
    }
}
