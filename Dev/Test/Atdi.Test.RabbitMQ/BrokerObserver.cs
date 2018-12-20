using Atdi.Modules.AmqpBroker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.RabbitMQ
{
    class BrokerObserver : IBrokerObserver
    {
        public void OnEvent(IBrokerEvent brokerEvent)
        {
            Console.WriteLine($"[{brokerEvent.Level.ToString()}][{brokerEvent.Context}] {brokerEvent.Text}");
        }
    }
}
