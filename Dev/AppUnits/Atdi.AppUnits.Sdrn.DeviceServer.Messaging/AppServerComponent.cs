using Atdi.Api.Sdrn.Device.BusController;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppServer;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging
{
    public class AppServerComponent : AppUnitComponent
    {

        public AppServerComponent() 
            : base("SdrnDeviceServerMessagingAppUnit")
        {
            
        }

        protected override void OnInstallUnit()
        {
            var exampleConfig = this.Config.Extract<ConfigMessaging>();
            this.Container.RegisterInstance(exampleConfig, ServiceLifetime.Singleton);
            this.Container.Register<Handlers.SendCommandHandler>(ServiceLifetime.Singleton);
            this.Container.Register<Handlers.SendMeasTaskHandler>(ServiceLifetime.Singleton);
            this.Container.Register<Handlers.SendRegistrationResultHandler>(ServiceLifetime.Singleton);
            this.Container.Register<Handlers.SendSensorUpdatingResultHandler>(ServiceLifetime.Singleton);
        }

        protected override void OnActivateUnit()
        {
            var gate = this.Resolver.Resolve<IBusGate>();
            var busEventObserver = this.Resolver.Resolve<IBusEventObserver>();
            var dispatcher = gate.CreateDispatcher("SDRN.DeviceServer", busEventObserver);

            var sendCommandHandler = this.Resolver.Resolve<Handlers.SendCommandHandler>();
            var sendMeasTaskHandler = this.Resolver.Resolve<Handlers.SendMeasTaskHandler>();
            var sendRegistrationResultHandler = this.Resolver.Resolve<Handlers.SendRegistrationResultHandler>();
            var sendSensorUpdatingResultHandler = this.Resolver.Resolve<Handlers.SendSensorUpdatingResultHandler>();

            dispatcher.RegistryHandler(sendCommandHandler);
            dispatcher.RegistryHandler(sendMeasTaskHandler);
            dispatcher.RegistryHandler(sendRegistrationResultHandler);
            dispatcher.RegistryHandler(sendSensorUpdatingResultHandler);

            this.Container.RegisterInstance<IMessageDispatcher>(dispatcher, ServiceLifetime.Singleton);

            var hostLoader = this.Resolver.Resolve<IServerHostLoader>();

            hostLoader.RegisterTrigger("Running device bus handlers activation", () =>
            {
                dispatcher.Activate();
            });
            
        }

        protected override void OnDeactivateUnit()
        {
        }
        protected override void OnUninstallUnit()
        {
        }
    }
}
