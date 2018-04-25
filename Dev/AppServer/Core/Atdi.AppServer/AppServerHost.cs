using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Castle.Facilities.WcfIntegration;
using Castle.Windsor;
using Atdi.AppServer.Models.TechServices;

namespace Atdi.AppServer
{
    public class AppServerHost : IDisposable
    {
        private readonly string _instanceName;
        private IWindsorContainer _container;
        private IAppServerContext _serverContext;
        private ILogger _logger;
        private List<IAppServerComponent> _components;
        private AppServerHostState _state;
        
        public AppServerHost(string instanceName, IReadOnlyList<IAppServerComponent> components)
        {
            this._state = AppServerHostState.Initializing;

            this._instanceName = instanceName;

            this._container = new WindsorContainer()
               .Install(new AppServerInstaller(this));

            this._logger = this._container.Resolve<ILogger>();

            this._logger.Info("AppServer: Server Host is initializing ...");
            this._logger.Info("AppServer: The DI-Container has been initialized.");
            this._logger.Info("AppServer: The Logger has been initialized.");

            this._serverContext = this._container.Resolve<IAppServerContext>();

            this._logger.Info("AppServer: The Server Context has been initialized.");

            this._components = new List<IAppServerComponent>();
            if (components != null && components.Count > 0)
            {
                this._components.AddRange(components);
                foreach (var component in this._components)
                {
                    component.Install(this._container, this._serverContext);
                }
            }
            this._logger.Info("AppServer: The Server Components has been installed.");

            this._state = AppServerHostState.Initialized;

            this._logger.Info("AppServer: Server Host has been initialized successfully.");
        }

        public static AppServerHost Create(string instanceName, IReadOnlyList<IAppServerComponent> components)
        {
            return new AppServerHost(instanceName, components);
        }

        public string InstanceName => this._instanceName;

        public void Introduce(IAppServerComponent component)
        {
            if (this._state == AppServerHostState.Disposed 
                || this._state == AppServerHostState.Initializing 
                || this._state == AppServerHostState.Starting 
                || this._state == AppServerHostState.Stopping
                || this._state == AppServerHostState.Disposing)
            {
                throw new InvalidOperationException("Incorrect current host state for introducing component");
            }

            this._components.Add(component);

            component.Install(this._container, this._serverContext);

            if (this._state == AppServerHostState.Started)
            {
                component.Activate();
            }
        }

        public void Start()
        {
            if (this._state == AppServerHostState.Started)
            {
                return;
            }

            if (this._state != AppServerHostState.Initialized && this._state != AppServerHostState.Stopped)
            {
                throw new InvalidOperationException("Incorrect current host state for starting");
            }

            this._logger.Info("AppServer: The Server Host is starting ...");
            try
            {
                foreach (var component in this._components)
                {
                    component.Activate();
                }

                this._logger.Info("AppServer: The Server Host has been started successfully");

            }
            catch (Exception e)
            {
                this._logger.Fatal(e);
                throw;
            }
        }

        public void Stop()
        {
            if (this._state == AppServerHostState.Stopped || this._state == AppServerHostState.Initialized)
            {
                return;
            }

            if (this._state != AppServerHostState.Started)
            {
                throw new InvalidOperationException("Incorrect current host state for stopping");
            }

            this._logger.Info("AppServere: The Server Host is stopping ...");
            try
            {
                foreach (var component in this._components)
                {
                    component.Deactivate();
                }

                this._logger.Info("AppServer: The Server Host has been stopped successfully");

            }
            catch (Exception e)
            {
                this._logger.Fatal(e);
                throw;
            }
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (this._state != AppServerHostState.Disposed && this._state != AppServerHostState.Disposing)
            {
                this._state = AppServerHostState.Disposing;

                if (disposing)
                {
                    this._logger.Info("AppServere: Stoping ...");

                    // TODO: dispose managed state (managed objects).
                    if (this._components != null)
                    {
                        foreach (var component in this._components)
                        {
                            component.Deactivate();
                        }
                        this._logger.Info("AppServer: The Server Components has been deactivating");

                        foreach (var component in this._components)
                        {
                            component.Uninstall(this._container, this._serverContext);
                        }

                        this._logger.Info("AppServer: The Server Components has been uninstalling");
                    }

                    if (this._container != null)
                    {
                        this._logger.Info("AppServer: The DI-Container has been closing");
                        this._logger.Info("AppServer: The Server Host has been stoping");
                        this._logger = null;
                        this._container.Dispose();
                        this._container = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                this._state = AppServerHostState.Disposed;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AppServerHost() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
