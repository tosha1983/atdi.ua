using ADP = Atdi.AppUnits.Sdrn.DeviceServer.Adapters;
using CMD = Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.UnitTest.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Sdrn.DeviceServer.Adapters
{
    class Program
    {
        static void Main(string[] args)
        {
            // подготовка тестового окружения
            var logger = new ConsoleLogger();
            var adapterHost = new DummyAdapterHost(logger);


            // конфигурация
            var adapterConfig = new ADP.ExampleAdapter.AdapterConfig()
            {
                Prop1 = 1,
                Prop2 = 2,
                Prop3 = 3,
                Prop4 = 4,
                Prop5 = 5
            };

            // проходим по фазам

            // to init
            var adapter = new ADP.ExampleAdapter.AdapterObject(adapterConfig, logger);

            // to connect
            adapter.Connect(adapterHost);

            // send command
            var context = new DummyExecutionContext(logger);
            var command = new CMD.MesureGpsLocationExampleCommand();
            command.Parameter.CountPerSec = 100;

            adapter.MesureGpsLocationExampleCommandHandler(command, context);

            // to disconnect
            adapter.Disconnect();

            Console.WriteLine("Press [Enter] for exit.");
            Console.ReadLine();
        }
    }
}
