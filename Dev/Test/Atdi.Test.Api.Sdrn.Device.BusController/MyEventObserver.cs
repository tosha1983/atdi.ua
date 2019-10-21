using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Api.Sdrn.Device.BusController
{
    class MyEventObserver : IBusEventObserver
    {
        public void OnEvent(IBusEvent busEvent)
        {
            var context = busEvent.Context;
            if (!string.IsNullOrEmpty(context))
            {
                var parts = busEvent.Context.Split('.');
                if (parts.Length == 2)
                {
                    context = $"{parts[0]}({parts[1]})";
                }
            }
            

            Console.WriteLine($"{busEvent.Created} [{busEvent.Level}] {context}: '{busEvent.Text}'");

        }
    }
}
