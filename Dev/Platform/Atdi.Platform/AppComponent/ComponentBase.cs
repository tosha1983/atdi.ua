using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;

namespace Atdi.Platform.AppServer
{
    public abstract class ComponentBase : IComponent
    {
        private IServicesContainer _container;
        private IServicesResolver _resolver;
        private IComponentConfig _config;
        private ILogger _logger;

        public ComponentBase(string name, ComponentType type, ComponentBehavior behavior)
        {
            this.Name = name;
            this.Type = type;
            this.Behavior = behavior;
        }
        public ComponentBehavior Behavior { get; private set; }

        public ComponentType Type { get; private set; }

        public string Name { get; private set; }

        public void Activate()
        {
            this.OnActivate();
            _logger.Debug(this.Name, this.Type.ToString(), (EventText)"Activated");
        }

        public void Deactivate()
        {
            this.OnDeactivate();
            _logger.Debug(this.Name, this.Type.ToString(), (EventText)"Deactivated");
        }

        public void Install(IServicesContainer container, IComponentConfig config)
        {
            this._container = container;
            this._config = config;
            this._resolver = container.GetResolver<IServicesResolver>();
            this._logger = this._resolver.Resolve<ILogger>();

            this.OnInstall();

            _logger.Debug(this.Name, this.Type.ToString(), (EventText)"Installed");
        }

        public void Uninstall()
        {
            this.OnUninstall();
            _logger.Debug(this.Name, this.Type.ToString(), (EventText)"Uninstalled");
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
