using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.AppServer;
using Atdi.Platform.Logging;

namespace Atdi.CoreServices.DataLayer
{
    public sealed class DataLayerComponent : IComponent
    {
        private IServicesContainer _container;
        private IServicesResolver _resolver;
        private IComponentConfig _config;
        private ILogger _logger;

        public ComponentBehavior Behavior => ComponentBehavior.Simple | ComponentBehavior.SingleInstance;

        public ComponentType Type => ComponentType.CoreServices;

        public string Name => "DataLayer";

        public void Activate()
        {
            _logger.Debug("DataLayer", "Activate");
        }

        public void Deactivate()
        {
            _logger.Debug("DataLayer", "Deactivate");
        }

        public void Install(IServicesContainer container, IComponentConfig config)
        {
            this._container = container;
            this._config = config;
            this._resolver = container.GetResolver<IServicesResolver>();
            this._logger = this._resolver.Resolve<ILogger>();

            _logger.Debug("DataLayer", "Install");
        }

        public void Uninstall()
        {
            _logger.Debug("DataLayer", "Uninstall");
            this._config = null;
            this._container = null;
        }
    }
}
