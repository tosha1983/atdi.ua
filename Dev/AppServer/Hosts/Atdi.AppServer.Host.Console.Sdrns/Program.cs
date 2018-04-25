using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.Sdrns;

using Atdi.AppServer.Services;
using Atdi.AppServer.Services.Sdrns;

using Atdi.AppServer.AppServices;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.AppServices.SdrnsController;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using Atdi.SDNRS.AppServer.Sheduler;
using Atdi.SDNRS.AppServer.BusManager;
using XMLLibrary;
using Atdi.AppServer.AppService.SdrnsController;
using Atdi.SDNRS.AppServer.ManageDB;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;
using Atdi.SDR.Server.Utils;
using EasyNetQ;

using Atdi.AppServer.CoreServices;
using Atdi.AppServer.CoreServices.DataLayer;
using Atdi.AppServer.CoreServices.DataLayer.MsSql;
using Atdi.AppServer.CoreServices.DataLayer.Oracle;
using Atdi.AppServer.RunServices;

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
            var dataLayerComponent = new DataLayerCoreServicesServerComponent();
            var msSqlComponent = new MsSqlDataLayerCoreServicesServerComponent();
            //var oracleComponent = new OracleLayerCoreServicesServerComponent();
            var sdrnsRunServiceComponent = new RunServiceComponent();


            components.Add(sdrnsControllerAppServiceHost);
            components.Add(sdrnsControllerWcfServiceHost);
            components.Add(coreServicesComponent);
            components.Add(dataLayerComponent);
            components.Add(msSqlComponent);
            ///components.Add(oracleComponent);
            components.Add(sdrnsRunServiceComponent);


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
