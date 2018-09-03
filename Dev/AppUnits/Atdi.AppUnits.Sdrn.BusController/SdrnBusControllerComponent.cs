using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.BusController
{
    public class SdrnBusControllerComponent : AppUnitComponent
    {
        private SdrnServerDescriptor _serverDescriptor;
        private ConsumersBusConnector _consumersConnector;

        public SdrnBusControllerComponent() 
            : base("SdrnBusControllerAppUnit")
        {
            
        }

        protected override void OnInstallUnit()
        {
            this._serverDescriptor = new SdrnServerDescriptor(this.Config);
            this.Container.RegisterInstance(this._serverDescriptor, ServiceLifetime.Singleton);
            this.Container.Register<ConsumersRabbitMQConnection>(ServiceLifetime.PerThread);
            this.Container.Register<PublisherRabbitMQConnection>(ServiceLifetime.PerThread);
            this.Container.Register<ConsumersBusConnector>(ServiceLifetime.Transient);
            this.Container.Register<PublisherBusConnector>(ServiceLifetime.PerThread);
        }

        protected override void OnActivateUnit()
        {
            this._consumersConnector = this.Resolver.Resolve<ConsumersBusConnector>();
        }

        protected override void OnDeactivateUnit()
        {
            if (this._consumersConnector != null)
            {
                this._consumersConnector.Dispose();
                this._consumersConnector = null;
            }
        }
        protected override void OnUninstallUnit()
        {
            if (this._consumersConnector != null)
            {
                this._consumersConnector.Dispose();
                this._consumersConnector = null;
            }
            this._serverDescriptor = null;
        }
    }
}
