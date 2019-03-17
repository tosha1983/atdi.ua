using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private ITypeResolver _typeResolver;
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
                for (int i = 0; i < componentsConfig.Length; i++)
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
            this.Logger.Verbouse(Contexts.AppServerHost, Categories.Installation, Events.ServerComponentIsInstalling.With(config.Type, config.Instance, config.Assembly));
            IComponent component = null;
            try
            {
                component = this._typeResolver.CreateInstance<IComponent>(new AssemblyName(config.Assembly));
                component.Install(this._container, config);
                this.Logger.Verbouse(Contexts.AppServerHost, Categories.Installation, Events.ServerComponentInstalled);
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

        //private readonly string _instanceName;
        //private IWindsorContainer _container;
        //private IAppServerContext _serverContext;
        //private ILogger _logger;
        //private List<IAppServerComponent> _components;
        //private AppServerHostState _state;

        //public Host(string instanceName, IReadOnlyList<IAppServerComponent> components)
        //{
        //    this._state = AppServerHostState.Initializing;

        //    this._instanceName = instanceName;

        //    this._container = new WindsorContainer()
        //       .Install(new Installer(this));

        //    this._logger = this._container.Resolve<ILogger>();

        //    this._logger.Info("AppServer: Server Host is initializing ...");
        //    this._logger.Info("AppServer: The DI-Container has been initialized.");
        //    this._logger.Info("AppServer: The Logger has been initialized.");

        //    this._serverContext = this._container.Resolve<IAppServerContext>();

        //    this._logger.Info("AppServer: The Server Context has been initialized.");

        //    this._components = new List<IAppServerComponent>();
        //    if (components != null && components.Count > 0)
        //    {
        //        this._components.AddRange(components);
        //        foreach (var component in this._components)
        //        {
        //            component.Install(this._container, this._serverContext);
        //        }
        //    }
        //    this._logger.Info("AppServer: The Server Components has been installed.");

        //    this._state = AppServerHostState.Initialized;

        //    this._logger.Info("AppServer: Server Host has been initialized successfully.");
        //}

        //public static Host Create(string instanceName, IReadOnlyList<IAppServerComponent> components)
        //{
        //    return new Host(instanceName, components);
        //}

        //public string InstanceName => this._instanceName;

        //public void Introduce(IAppServerComponent component)
        //{
        //    if (this._state == AppServerHostState.Disposed 
        //        || this._state == AppServerHostState.Initializing 
        //        || this._state == AppServerHostState.Starting 
        //        || this._state == AppServerHostState.Stopping
        //        || this._state == AppServerHostState.Disposing)
        //    {
        //        throw new InvalidOperationException("Incorrect current host state for introducing component");
        //    }

        //    this._components.Add(component);

        //    component.Install(this._container, this._serverContext);

        //    if (this._state == AppServerHostState.Started)
        //    {
        //        component.Activate();
        //    }
        //}

        //public void Start()
        //{
        //    if (this._state == AppServerHostState.Started)
        //    {
        //        return;
        //    }

        //    if (this._state != AppServerHostState.Initialized && this._state != AppServerHostState.Stopped)
        //    {
        //        throw new InvalidOperationException("Incorrect current host state for starting");
        //    }

        //    this._logger.Info("AppServer: The Server Host is starting ...");
        //    try
        //    {
        //        foreach (var component in this._components)
        //        {
        //            component.Activate();
        //        }

        //        this._logger.Info("AppServer: The Server Host has been started successfully");

        //    }
        //    catch (Exception e)
        //    {
        //        this._logger.Fatal(e);
        //        throw;
        //    }
        //}

        //public void Stop()
        //{
        //    if (this._state == AppServerHostState.Stopped || this._state == AppServerHostState.Initialized)
        //    {
        //        return;
        //    }

        //    if (this._state != AppServerHostState.Started)
        //    {
        //        throw new InvalidOperationException("Incorrect current host state for stopping");
        //    }

        //    this._logger.Info("AppServere: The Server Host is stopping ...");
        //    try
        //    {
        //        foreach (var component in this._components)
        //        {
        //            component.Deactivate();
        //        }

        //        this._logger.Info("AppServer: The Server Host has been stopped successfully");

        //    }
        //    catch (Exception e)
        //    {
        //        this._logger.Fatal(e);
        //        throw;
        //    }
        //}

        //#region IDisposable Support

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (this._state != AppServerHostState.Disposed && this._state != AppServerHostState.Disposing)
        //    {
        //        this._state = AppServerHostState.Disposing;

        //        if (disposing)
        //        {
        //            this._logger.Info("AppServere: Stoping ...");

        //            // TODO: dispose managed state (managed objects).
        //            if (this._components != null)
        //            {
        //                foreach (var component in this._components)
        //                {
        //                    component.Deactivate();
        //                }
        //                this._logger.Info("AppServer: The Server Components has been deactivating");

        //                foreach (var component in this._components)
        //                {
        //                    component.Uninstall(this._container, this._serverContext);
        //                }

        //                this._logger.Info("AppServer: The Server Components has been uninstalling");
        //            }

        //            if (this._container != null)
        //            {
        //                this._logger.Info("AppServer: The DI-Container has been closing");
        //                this._logger.Info("AppServer: The Server Host has been stoping");
        //                this._logger = null;
        //                this._container.Dispose();
        //                this._container = null;
        //            }
        //        }

        //        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        //        // TODO: set large fields to null.

        //        this._state = AppServerHostState.Disposed;
        //    }
        //}

        //// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        //// ~AppServerHost() {
        ////   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        ////   Dispose(false);
        //// }

        //// This code added to correctly implement the disposable pattern.
        //public void Dispose()
        //{
        //    // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //    Dispose(true);
        //    // TODO: uncomment the following line if the finalizer is overridden above.
        //    // GC.SuppressFinalize(this);
        //}
        //#endregion

        public void Dispose()
        {
            if (this._state == HostState.Disposed || this._state == HostState.Disposing)
            {
                return;
            }

            this.Logger.Info(Contexts.AppServerHost, Categories.Disposabling, Events.ServerHostIsDisposabling);
            var curState = this._state;
            this._state = HostState.Disposing;
            if (this._components != null)
            {
                if (curState == HostState.Started)
                {
                    this.DeactivateComponents();
                }
                this.UninstallComponents();
                this._components = null;
            }
            this._state = HostState.Disposed;
            this.Logger.Info(Contexts.AppServerHost, Categories.Disposabling, Events.ServerHostDisposabled);
            

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
            this._loader.ExecuteTruggers();
            this._state = HostState.Started;
            this.Logger.Info(Contexts.AppServerHost, Categories.Starting, Events.ServerHostStarted);


        }

        public void Stop()
        {
            if (this._state == HostState.Stopped || this._state == HostState.Initialized)
            {
                return;
            }

            if (this._state != HostState.Started)
            {
                throw new InvalidOperationException(Exceptions.IncorrectStateForStopping);
            }

            this.Logger.Info(Contexts.AppServerHost, Categories.Stopping, Events.ServerHostIsStoping);
            this._state = HostState.Stopping;
            this.DeactivateComponents();
            this._state = HostState.Stopped;
            this.Logger.Info(Contexts.AppServerHost, Categories.Stopping, Events.ServerHostStopped);
        }

        private void ActivateComponents()
        {
            this.Logger.Info(Contexts.AppServerHost, Categories.Starting, Events.ServerComponentsIsActivating);
            try
            {
                foreach (var descriptor in this._components)
                {
                    if ((descriptor.Component.Behavior & ComponentBehavior.WithoutActivation) != ComponentBehavior.WithoutActivation)
                    {
                        descriptor.Component.Activate();
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
                foreach (var descriptor in this._components)
                {
                    if ((descriptor.Component.Behavior & ComponentBehavior.WithoutActivation) != ComponentBehavior.WithoutActivation)
                    {
                        descriptor.Component.Deactivate();
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
            this.Logger.Info(Contexts.AppServerHost, Categories.Disposabling, Events.ServerComponentsIsUninstalling);
           
            foreach (var descriptor in this._components)
            {
                try
                {
                    descriptor.Component.Uninstall();
                }
                catch (Exception e)
                {
                    this.Logger.Exception(Contexts.AppServerHost, Categories.Stopping, e);
                }
            }

            this.Logger.Info(Contexts.AppServerHost, Categories.Disposabling, Events.ServerComponentsUninstalled);
        }
    }
}
