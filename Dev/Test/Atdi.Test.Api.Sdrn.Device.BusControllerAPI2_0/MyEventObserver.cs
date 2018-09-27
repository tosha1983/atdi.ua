using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Api.Sdrn.Device.BusControllerAPI2_0
{
    class MyEventObserver : IBusEventObserver
    {
        public void OnEvent(IBusEvent busEvent)
        {
            Console.WriteLine($"{busEvent.Created} :  Bus event - {busEvent.Level} - {busEvent.Source} - {busEvent.Context} - '{busEvent.Text}' - '{busEvent.Source}'");

        }
    }
}
