using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Atdi.Platform;

namespace Atdi.AppServer.Sdrn.DeviceServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Press any key to start SDRN App Server (AK) ...");
            //Console.ReadLine();

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

                Console.WriteLine($"Press any key to stop SDRN App Server (AK) ...");
                Console.ReadLine();

                try
                {
                    host.Stop("Normal completion");
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
