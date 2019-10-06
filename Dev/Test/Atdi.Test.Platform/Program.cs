using Atdi.Platform;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Platform
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Press any key to start ATDI Platform ...");
            Console.ReadLine();

            using (var host = PlatformConfigurator.BuildHost())
            {
                try
                {
                    host.Start();
                    var resolver = host.Container.GetResolver<IServicesResolver>();

                    var logger = resolver.Resolve<ILogger>();
                    
                    //var calc = new CalclCrowed(logger);
                    // DataLayerTest.Run(resolver);
                    // EntityOrmTest.Run(resolver);

                    LoggerTest.Run(logger);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }

                Console.WriteLine($"Press any key to stop SDRN App Server (AK) ...");
                Console.ReadLine();

                try
                {
                    host.Stop();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
            }

            Console.WriteLine($"Server host was stopped. Press any key to exit ...");
            Console.ReadLine();
        }
    }
}
