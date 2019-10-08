using System.Collections.Generic;
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
            //_logger.Debug("ServerHost", "Activating", (EventText)$"The component is being activated: {this}");

            this.OnActivate();

            _logger.Info("Component", "Activating", (EventText)$"The component has activated: {this}");
        }

        public void Deactivate()
        {
            this.OnDeactivate();
            _logger.Info("Component", "Deactivating", (EventText)$"The component has deactivated: {this}");
        }

        public void Install(IServicesContainer container, IComponentConfig config)
        {
            this._container = container;
            this._config = config;
            this._resolver = container.GetResolver<IServicesResolver>();
            this._logger = this._resolver.Resolve<ILogger>();


            _logger.Info("Component", "Loading", (EventText)$"The component has loaded: {this}");
            _logger.Debug("Component", "Loading", (EventText)$"The component data", new Dictionary<string, object>
            {
                ["Assembly"] = this.GetType().Assembly.FullName,
                ["Type"] = this.Type,
                ["Name"] = this.Name,
                ["Behavior"] = this.Behavior
            });

            this.OnInstall();
        }

        public void Uninstall()
        {
            this.OnUninstall();
            _logger.Info("Component", "Unloading", (EventText)$"The component has unloaded: {this}");

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

        public override string ToString()
        {
            var name = this.GetType().Assembly.GetName();
            return $"Name='{name.Name}', Version='{name.Version}'";
        }
    }
}
