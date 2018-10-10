using Atdi.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Atdi.AppUnits.Sdrn.Server.PrimaryHandlers;

namespace Atdi.AppServer.Sdrn.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Press any key to start SDRN Server ...");
            Console.ReadLine();

            using (var host = PlatformConfigurator.BuildHost())
            {
                try
                {
                    host.Start();
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }

                Console.ReadLine();
                host.Stop();

            }
            Console.ReadLine();
        }
    }
}
