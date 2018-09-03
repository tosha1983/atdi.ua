using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform;

namespace Atdi.AppServer.WebQuery
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //var s = "12, 66,77,   988 3".Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);

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
