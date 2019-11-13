using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform;

namespace Atdi.AppServer.WebQuery.Console.AK
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
                    System.Console.WriteLine("Exception: " + e.Message);
                }

                System.Console.ReadKey();

                host.Stop();

            }
            System.Console.ReadKey();
        }
    }
}
