using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.Facilities.TypedFactory;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Common;
using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppServer.AppServices;
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
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.AppComponent;
using System.Configuration;
using Atdi.CoreServices.DataLayer;

namespace Atdi.AppServer.RunServices
{ 
    public class RunServiceComponent : IAppServerComponent
    {
        private readonly string _name;
        private ILogger _logger;



        public RunServiceComponent()
        {
            this._name = "RunServiceComponent";
        }

        AppServerComponentType IAppServerComponent.Type => AppServerComponentType.AppService;

        string IAppServerComponent.Name => this._name;

        void IAppServerComponent.Activate()
        {
            Configuration conf = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            InitConnectionString.oraDbString = ConfigurationManager.ConnectionStrings["ORACLE_DB_ICSM_ConnectionString"].ConnectionString;
            BaseXMLConfiguration xml_conf = new BaseXMLConfiguration();
            GlobalInit.Initialization();
            ClassesDBGetResult DbGetRes = new ClassesDBGetResult(_logger);
            ClassConvertToSDRResults conv = new ClassConvertToSDRResults(_logger);
            ShedulerUpMeasSDRResults Sc_Up_Meas_SDR = new ShedulerUpMeasSDRResults(_logger);
            Sc_Up_Meas_SDR.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._TimeUpdateMeasResult);
            ShedulerCheckActivitySensor CheckActivitySensor = new ShedulerCheckActivitySensor(_logger);
            CheckActivitySensor.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._RescanActivitySensor);
            ShedulerGetMeasTask getMeasTask = new ShedulerGetMeasTask(this._logger); getMeasTask.ShedulerRepeatStart(20);
            ShedulerCheckStart Quartz = new ShedulerCheckStart(this._logger);
            Quartz.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._ReloadStart);
        }

        void IAppServerComponent.Deactivate()
        {
            ;
        }

        void IAppServerComponent.Install(IWindsorContainer container, IAppServerContext serverContext)
        {
            this._logger = container.Resolve<ILogger>();
        }

        void IAppServerComponent.Uninstall(IWindsorContainer container, IAppServerContext serverContext)
        {
            _logger.Trace("Component RunServiceComponent: Uninstalled");

        }

    }
}
