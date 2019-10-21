using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform;
using Atdi.Platform.Logging;
using System.ServiceModel;


namespace Atdi.AppServer.WebQuery
{
    class Program
    {
        static void Main(string[] args)
        {

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

                Console.ReadKey();

                host.Stop();

            }
            Console.ReadKey();
        }
    }
}
