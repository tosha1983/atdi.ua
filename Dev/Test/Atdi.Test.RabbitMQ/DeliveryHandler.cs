using Atdi.Modules.AmqpBroker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.RabbitMQ
{
    class DeliveryHandler : IDeliveryHandler
    {
        public HandlingResult Handle(IDeliveryMessage message, IDeliveryContext deliveryContext)
        {
            var tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($" ------ [recieved message][{tid}] : {message.Type} '{message.Id}', size {message.Body.Length}");
            return HandlingResult.Confirm;

            
            
            //    txtMsg.AppendText($"#{tid} >>>[Channel #{deliveryContext.Channel.Number}].Consumer: '{deliveryContext.ConsumerTag}'; DeliveryTag: '{deliveryContext.DeliveryTag}'; Exchange: '{deliveryContext.Exchange}'; RoutingKey: '{deliveryContext.RoutingKey}'; AppID: '{message.AppId}'; Message ID: '{message.Id}'" + Environment.NewLine);

           
        }
    }
}
