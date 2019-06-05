using Atdi.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Sdrn.Device.WcfService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Press any key to start SDRN Device WCF Service App Server Host (AK config) ...");
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

                Console.WriteLine($"Press any key to stop SDRN Device WCF Service App Server Host (AK config) ...");
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

            Console.WriteLine($"App Server Host was stopped. Press any key to exit ...");
            Console.ReadLine();
        }
    }
}
