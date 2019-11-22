using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.Platform.Logging;
using Atdi.Platform.DependencyInjection;
using System.Reflection;
using Atdi.Platform.AppComponent;

namespace Atdi.Platform.AppServer
{
    public class ServerHost : LoggedObject, IServerHost
    {
        private IServicesContainer _container;
        private IServerContext _serverContext;
        private List<ComponentDescriptor> _components;
        private HostState _state;
        private readonly ITypeResolver _typeResolver;
        private ServerHostLoader _loader;

        public ServerHost(ILogger logger, IServicesContainer container, IServerConfig config, ITypeResolver typeResolver) : base(logger)
        {
            using (this.Logger.StartTrace(Contexts.AppServerHost, Categories.Initialization, TraceScopeNames.Constructor))
            {
                this.Logger.Info(Contexts.AppServerHost, Categories.Initialization, Events.StartedInitServerHost);
                this._container = container;
                this._typeResolver = typeResolver;
                this._state = HostState.Initializing;
                
                this.PrepareServerContext(config);
                this.PrepareServerHostLoader();
                this.InstallComponents(config.Components);

                this._state = HostState.Initialized;
                this.Logger.Info(Contexts.AppServerHost, Categories.Initialization, Events.CreatedServerHost.With(config.Instance));
            }
        }

        private void PrepareServerHostLoader()
        {
            this._loader = new ServerHostLoader(this.Logger);
            this._container.RegisterInstance<IServerHostLoader>(this._loader, ServiceLifetime.Singleton);
        }

        private void PrepareServerContext(IServerConfig config)
        {
            this._serverContext = new ServerContext(config);
            this._container.RegisterInstance<IServerContext>(this._serverContext);
        }

        private void InstallComponents(IComponentConfig[] componentsConfig)
        {
            this.Logger.Info(Contexts.AppServerHost, Categories.Initialization, Events.ServerComponentsIsInstalling);

            if (componentsConfig != null && componentsConfig.Length > 0)
            {
                this._components = new List<ComponentDescriptor>(componentsConfig.Length);
                for (var i = 0; i < componentsConfig.Length; i++)
                {
                    var descriptor = InstallComponent(componentsConfig[i]);
                    if (descriptor != null)
                    {
                        this._components.Add(descriptor);
                    }
                }
            }
            else
            {
                this._components = new List<ComponentDescriptor>();
            }

            this.Logger.Info(Contexts.AppServerHost, Categories.Initialization, Events.ServerComponentsInstalled.With(this._components.Count));
        }

        private ComponentDescriptor InstallComponent(IComponentConfig config)
        {
            this.Logger.Info(Contexts.AppServerHost, Categories.Installation, Events.ServerComponentIsInstalling.With(config.Type, config.Instance, config.Assembly));
            try
            {
                var parameters = config.Parameters.ToDictionary(k=>k.Name, v=>(object)v.Value);

                this.Logger.Debug(Contexts.AppServerHost, Categories.Installation, (EventText)$"Config parameters ({parameters.Count})", parameters);

                var component = this._typeResolver.CreateInstance<IComponent>(new AssemblyName(config.Assembly));
                component.Install(this._container, config);
                this.Logger.Info(Contexts.AppServerHost, Categories.Installation, Events.ServerComponentInstalled.With(config.Type, config.Instance, config.Assembly));

                return new ComponentDescriptor(component, config);
            }
            catch(Exception e)
            {
                this.Logger.Exception(Contexts.AppServerHost, Categories.Installation, e);
                this.Logger.Error(Contexts.AppServerHost, Categories.Installation, Events.ServerComponentDidNotInstall.With(config.Type, config.Instance, config.Assembly));
                return null;
            }
            
        }

        public HostState State => this._state;


        public void Dispose()
        {
            if (this._state == HostState.Disposed || this._state == HostState.Disposing)
            {
                return;
            }
            var curState = this._state;
            if (curState == HostState.Started)
            {
                this.Stop("Unexpected completion");
            }

            this.Logger.Info(Contexts.AppServerHost, Categories.Disposing, Events.ServerHostIsDisposing);
            
            this._state = HostState.Disposing;
            if (this._components != null)
            {
                
                this.UninstallComponents();
                this._components = null;
            }
            this._state = HostState.Disposed;
            this.Logger.Info(Contexts.AppServerHost, Categories.Disposing, Events.ServerHostDisposed);
            

            if (this._container != null)
            {
                this._container.Dispose();
                this._container = null;
            }
        }

        public void Start()
        {
            if (this._state == HostState.Started)
            {
                return;
            }

            if (this._state != HostState.Initialized && this._state != HostState.Stopped)
            {
                throw new InvalidOperationException(Exceptions.IncorrectStateForStarting);
            }

            this.Logger.Info(Contexts.AppServerHost, Categories.Starting, Events.ServerHostIsStarting);
            this._state = HostState.Starting;

            this.ActivateComponents();
            
            this._state = HostState.Started;
            this.Logger.Info(Contexts.AppServerHost, Categories.Starting, Events.ServerHostStarted);

            this._loader.ExecuteTriggers();
        }

        public void Stop()
        {
            this.Stop("Unknown");
        }

        public void Stop(string reason)
        {
            if (this._state == HostState.Stopped || this._state == HostState.Initialized)
            {
                return;
            }

            if (this._state != HostState.Started)
            {
                throw new InvalidOperationException(Exceptions.IncorrectStateForStopping);
            }

            this.Logger.Info(Contexts.AppServerHost, Categories.Stopping, Events.ServerHostIsStopping.With(reason));
            this._state = HostState.Stopping;
            this.DeactivateComponents();
            this._state = HostState.Stopped;
            this.Logger.Info(Contexts.AppServerHost, Categories.Stopping, Events.ServerHostStopped.With(reason));
        }

        private void ActivateComponents()
        {
            this.Logger.Info(Contexts.AppServerHost, Categories.Starting, Events.ServerComponentsIsActivating);
            try
            {
                if (this._components != null)
                {
                    foreach (var descriptor in this._components)
                    {
                        if ((descriptor.Component.Behavior & ComponentBehavior.WithoutActivation) != ComponentBehavior.WithoutActivation)
                        {
                            try
                            {
                                descriptor.Component.Activate();
                            }
                            catch (Exception e)
                            {
                                this.Logger.Exception(Contexts.AppServerHost, Categories.Starting, e);
                                this.Logger.Error(Contexts.AppServerHost, Categories.Starting, $"Can not activate the component '{descriptor.Component.Name}'");
                            }

                        }
                    }
                }

                this.Logger.Info(Contexts.AppServerHost, Categories.Starting, Events.ServerComponentsActivated);

            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.AppServerHost, Categories.Starting, e);
                throw;
            }
        }

        private void DeactivateComponents()
        {
            this.Logger.Info(Contexts.AppServerHost, Categories.Stopping, Events.ServerComponentsIsDeactivating);
            try
            {
                if (this._components != null)
                {
                    // двигаемся в обратном порядке
                    for (var i = this._components.Count - 1; i >= 0; i--)
                    {
                        var descriptor = this._components[i];
                        if ((descriptor.Component.Behavior & ComponentBehavior.WithoutActivation) != ComponentBehavior.WithoutActivation)
                        {
                            descriptor.Component.Deactivate();
                        }
                    }
                }

                this.Logger.Info(Contexts.AppServerHost, Categories.Stopping, Events.ServerComponentsDeactivated);

            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.AppServerHost, Categories.Stopping, e);
                throw;
            }
        }

        private void UninstallComponents()
        {
            this.Logger.Info(Contexts.AppServerHost, Categories.Disposing, Events.ServerComponentsIsUninstalling);

            // двигаемся в обратном порядке
            for (var i = this._components.Count - 1; i >= 0; i--)
            {
                var descriptor = this._components[i];
                try
                {
                    descriptor.Component.Uninstall();
                }
                catch (Exception e)
                {
                    this.Logger.Exception(Contexts.AppServerHost, Categories.Stopping, e);
                }
            }

            this.Logger.Info(Contexts.AppServerHost, Categories.Disposing, Events.ServerComponentsUninstalled);
        }

        public IServicesContainer Container => this._container;

        public IServerContext Context => this._serverContext;
    }
}
