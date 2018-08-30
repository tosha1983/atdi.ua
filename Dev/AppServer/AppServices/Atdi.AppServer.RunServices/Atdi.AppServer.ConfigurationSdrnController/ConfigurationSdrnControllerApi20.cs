using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.Facilities.TypedFactory;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Configuration;


namespace Atdi.AppServer.RunServices
{
    public class ConfigurationSdrnControllerApi20 : IAppServerComponent
    {
        private readonly string _name;
        private ILogger _logger;



        public ConfigurationSdrnControllerApi20()
        {
            this._name = "ConfigurationSdrnControllerApi20Component";
        }

        AppServerComponentType IAppServerComponent.Type => AppServerComponentType.AppService;

        string IAppServerComponent.Name => this._name;

        void IAppServerComponent.Activate()
        {

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
