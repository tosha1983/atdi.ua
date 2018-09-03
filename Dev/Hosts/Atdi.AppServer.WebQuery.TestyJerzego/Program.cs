using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using Atdi.Platform;

//using OrmCs;

namespace Atdi.AppServer.WebQuery.TestyJerzego
{
    class Program
    {
        //static void CreateTousandUsers()
        //{
        //    for(int i; i<1000; i++ )
        //}
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
