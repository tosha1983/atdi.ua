using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;
using Atdi.DataModels.CoverageCalculation;
using Atdi.Contracts.WcfServices.Identity;
using Atdi.Contracts.WcfServices.WebQuery;
using Atdi.DataModels;
using Atdi.DataModels.Identity;
using System.ServiceModel;
using Atdi.WebQuery.CoverageCalculation;


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
                    var resolver = host.Container.GetResolver<IServicesResolver>();
                    var logger = resolver.Resolve<ILogger>();
                    var task = new StartTask(logger);
                    task.Run();
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
