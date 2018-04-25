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

namespace Atdi.AppServer.CoreServices.DataLayer.MsSql
{
    public class MsSqlDataLayerCoreServicesServerComponent : IAppServerComponent
    {
        private ILogger _logger;

        AppServerComponentType IAppServerComponent.Type => AppServerComponentType.CoreServices;

        string IAppServerComponent.Name => "MsSqlDataLayer";

        void IAppServerComponent.Activate()
        {
            _logger.Trace("Component MS SQL DataLayer: Activated");
        }

        void IAppServerComponent.Deactivate()
        {
            _logger.Trace("Component MS SQL DataLayer: Deactivated");
        }

        void IAppServerComponent.Install(IWindsorContainer container, IAppServerContext serverContext)
        {
            this._logger = container.Resolve<ILogger>();

            container.Register(
                    Component.For<IConstraintStatementBuilder>()
                        .ImplementedBy<IConstraintStatementBuilder>()
                        .LifeStyle.Singleton
                );

            _logger.Trace("Component MS SQL DataLayer: Installed");
        }

        void IAppServerComponent.Uninstall(IWindsorContainer container, IAppServerContext serverContext)
        {
            _logger.Trace("Component MS SQL DataLayer: Uninstalled");
            this._logger = null;
        }
    }
}
