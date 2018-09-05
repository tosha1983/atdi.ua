using System;
using System.Collections.Generic;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppServer.Services;
using Atdi.AppServer.Services.Sdrns;
using Atdi.AppServer.AppServices;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.AppServices.SdrnsController;
using Atdi.AppServer.CoreServices;

namespace Atdi.AppServer.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var components = new List<IAppServerComponent>();
            var sdrnsControllerAppServiceHost = new AppServiceHostServerComponent<SdrnsControllerAppService, GetMeasTaskAppOperationHandler>();
            var sdrnsControllerWcfServiceHost = new WcfServiceHostServerComponent<ISdrnsController, SdrnsControllerService>();
            var coreServicesComponent = new CoreServicesServerComponent();
            var sdrnsConfigurationController = new ConfigurationSdrnController.ConfigurationSdrnController();
            components.Add(sdrnsControllerAppServiceHost);
            components.Add(sdrnsControllerWcfServiceHost);
            components.Add(coreServicesComponent);
            components.Add(sdrnsConfigurationController);
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
