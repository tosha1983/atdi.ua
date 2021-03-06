﻿using Atdi.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Sdrn.DeviceService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Press any key to start SDRN Device WCF Service Host ...");
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
