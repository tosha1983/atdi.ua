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

namespace Atdi.AppServer.CoreServices.DataLayer
{
    public class DataLayerCoreServicesServerComponent : IAppServerComponent
    {
        private ILogger _logger;

        AppServerComponentType IAppServerComponent.Type => AppServerComponentType.CoreServices;

        string IAppServerComponent.Name => "DataLayer";

        void IAppServerComponent.Activate()
        {
            _logger.Trace("Component DataLayer: Activated");
        }

        void IAppServerComponent.Deactivate()
        {
            _logger.Trace("Component DataLayer: Deactivated");
        }

        void IAppServerComponent.Install(IWindsorContainer container, IAppServerContext serverContext)
        {
            this._logger = container.Resolve<ILogger>();
            _logger.Trace("Component DataLayer: Installed");
        }

        void IAppServerComponent.Uninstall(IWindsorContainer container, IAppServerContext serverContext)
        {
            _logger.Trace("Component DataLayer: Uninstalled");
            this._logger = null;
        }
    }
}
