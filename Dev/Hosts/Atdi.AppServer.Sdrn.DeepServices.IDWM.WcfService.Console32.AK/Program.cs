using Atdi.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Sdrn.DeepServices.IDWM
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine($"Press any key to start SDRN DeepService IDWM WCF Service App Server (32-bit host) ...");
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

				//Thread.Sleep(30000);

				Console.WriteLine($"Press any key to stop SDRN DeepService IDWM WCF Service App Server (32-bit host) ...");
				Console.ReadLine();

				host.Stop("Normal completion");

			}
			Console.ReadLine();
		}
	}
}
