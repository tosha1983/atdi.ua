using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Api.DataBus
{
    class BusEventObserver : IBusEventObserver
    {
        private readonly object _locker = new object();

        public void OnEvent(IBusEvent busEvent)
        {
            var message = this.Format(busEvent);
            lock(_locker)
            {
                if (busEvent.Level == BusEventLevel.Critical)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                if (busEvent.Level == BusEventLevel.Error)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }
                if (busEvent.Level == BusEventLevel.Exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                if (busEvent.Level == BusEventLevel.Verbouse)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                if (busEvent.Level == BusEventLevel.Info)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (busEvent.Level == BusEventLevel.Warning)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                Console.WriteLine(message);
                Console.ResetColor();
                
                
            }
        }

        private string Format(IBusEvent busEvent)
        {
            return busEvent.ToString();
        }
    }
}
