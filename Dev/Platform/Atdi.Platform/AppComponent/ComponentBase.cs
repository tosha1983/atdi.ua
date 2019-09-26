using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;

namespace Atdi.Platform.AppComponent
{
    public abstract class ComponentBase : IComponent
    {
        private IServicesContainer _container;
        private IServicesResolver _resolver;
        private IComponentConfig _config;
        private ILogger _logger;

        protected ComponentBase(string name, ComponentType type, ComponentBehavior behavior)
        {
            this.Name = name;
            this.Type = type;
            this.Behavior = behavior;
        }
        public ComponentBehavior Behavior { get; }

        public ComponentType Type { get; }

        public string Name { get; }

        public void Activate()
        {
            _logger.Debug("AppServer Host", "Activating", (EventText)$"The server component is activating: Name = '{this.Name}'");

            this.OnActivate();

            _logger.Debug("AppServer Host", "Activating", (EventText)$"The server component was activated: Name = '{this.Name}'");
        }

        public void Deactivate()
        {
            this.OnDeactivate();
            _logger.Debug("AppServer Host", "Deactivating", (EventText)$"The component {this.Name} was deactivated.");
        }

        public void Install(IServicesContainer container, IComponentConfig config)
        {
            this._container = container;
            this._config = config;
            this._resolver = container.GetResolver<IServicesResolver>();
            this._logger = this._resolver.Resolve<ILogger>();

            _logger.Debug("AppServer Host", "Installation", (EventText)$"The server component is installing: Name = '{this.Name}', Type = '{this.Type.ToString()}'");

            this.OnInstall();

            _logger.Debug("AppServer Host", "Installation", (EventText)$"The server component was installed: Name = '{this.Name}', Type = '{this.Type.ToString()}'");
        }

        public void Uninstall()
        {
            this.OnUninstall();
            _logger.Debug("AppServer Host", "Uninstallation", (EventText)$"The component {this.Name} was uninstalled.");
            this._config = null;
            this._container = null;
        }

        protected virtual void OnInstall(){ }
        protected virtual void OnUninstall() { }
        protected virtual void OnActivate() { }
        protected virtual void OnDeactivate() { }

        protected IServicesContainer Container => this._container;
        protected IServicesResolver Resolver => this._resolver;
        protected ILogger Logger => this._logger;
        protected IComponentConfig Config => this._config;

    }
}
