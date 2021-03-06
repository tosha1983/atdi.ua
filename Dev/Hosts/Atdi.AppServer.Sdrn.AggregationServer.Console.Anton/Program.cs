﻿using Atdi.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Sdrn.AggregationServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine($"Press any key to start SDRN Aggregation Server (Anton) ...");
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

                Console.WriteLine($"Press any key to stop SDRN Aggregation Server (Anton) ...");
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
