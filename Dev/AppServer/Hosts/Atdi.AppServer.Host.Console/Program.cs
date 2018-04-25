using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.WebQuery;

using Atdi.AppServer.Services;
using Atdi.AppServer.Services.WebQuery;

using Atdi.AppServer.AppServices;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.AppServices.WebQueryManager;

namespace Atdi.AppServer.Hosts
{
    class Program
    {
        static void Main(string[] args)
        {
            var components = new List<IAppServerComponent>();
            var webQueryManagerAppServiceHost = new AppServiceHostServerComponent<WebQueryManagerAppService, AuthenticateUserAppOperationHandler>();
            //var webQueryManagerAppServiceGetTree = new AppServiceHostServerComponent<WebQueryManagerAppService, GetQueryTreeAppOperationHandler>();

            var webQueryHost = new WcfServiceHostServerComponent<IWebQueryManager, WebQueryManagerService>();

            components.Add(webQueryManagerAppServiceHost);
            components.Add(webQueryHost);
            

            using (var serverHost = AppServerHost.Create("WebQueryAppServer", components))
            {
                try
                {
                    serverHost.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }

                Console.WriteLine("Press enter to stop the server host");
                Console.ReadKey();

                serverHost.Stop();
            }

            Console.WriteLine("Press enter for exit");
            Console.ReadKey();
        }
    }
}
