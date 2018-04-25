using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Castle.Facilities.WcfIntegration;
using Castle.Windsor;
using Castle.MicroKernel.Registration;

namespace Atdi.AppServer.Services
{
    public class WcfServiceHostServerComponent<TContract, TImplementedBy> : IAppServerComponent
        where TContract : class
        where TImplementedBy : TContract
    {
        private readonly string _name;
        private ILogger _logger;
        private DefaultServiceHostFactory _hostFactory;
        private ServiceHostBase _serviceHost;

        public WcfServiceHostServerComponent()
        {
            this._name = "WcfServiceHostServerComponent." + typeof(TContract).Name;
        }
        AppServerComponentType IAppServerComponent.Type => AppServerComponentType.TechService;

        string IAppServerComponent.Name => this._name;

        void IAppServerComponent.Activate()
        {
            try
            {
                if (_serviceHost == null)
                {
                    throw new InvalidOperationException("The service host has not been initialized");
                }

                if (this._serviceHost.State == CommunicationState.Opened)
                {
                    return;
                }

                if (this._serviceHost.State == CommunicationState.Created || this._serviceHost.State == CommunicationState.Closed)
                {
                    _serviceHost.Open();
                    this._logger.Info("The WCF Service Host '{0}' has been opened successfully", this._name);
                }
            }
            catch (Exception e)
            {
                this._logger.Error(e, this._name);
            }
        }

        void IAppServerComponent.Deactivate()
        {
            try
            {
                if (_serviceHost == null)
                {
                    throw new InvalidOperationException("The service host has not been initialized");
                }

                if (this._serviceHost.State == CommunicationState.Closed || this._serviceHost.State == CommunicationState.Created)
                {
                    return;
                }

                if (this._serviceHost.State == CommunicationState.Opened || this._serviceHost.State == CommunicationState.Opening)
                {
                    _serviceHost.Close();
                    this._logger.Info("The WCF Service Host '{0}' has been closed successfully", this._name);
                }
            }
            catch (Exception e)
            {
                this._logger.Error(e, this._name);
            }
        }

        void IAppServerComponent.Install(IWindsorContainer container, IAppServerContext serverContext)
        {
            this._logger = container.Resolve<ILogger>();

            container.AddFacility<WcfFacility>();

            container.Register(
                    Component.For<TContract>()
                        .ImplementedBy<TImplementedBy>()
                        .LifestylePerWcfOperation()
                );

            this._hostFactory = new DefaultServiceHostFactory(container.Kernel);
            this._serviceHost = this._hostFactory.CreateServiceHost(typeof(TContract).AssemblyQualifiedName, new Uri[0]);
        }

        void IAppServerComponent.Uninstall(IWindsorContainer container, IAppServerContext serverContext)
        {
            
        }
    }
}
