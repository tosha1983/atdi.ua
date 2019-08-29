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
using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.DeviceBusHandlers;

namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement
{
    public class AppServerComponent : AppUnitComponent
    {

        public AppServerComponent() 
            : base("SdrnDeviceServerOnlineMeasurementAppUnit")
        {
            
        }

        protected override void OnInstallUnit()
        {
            var unitConfig = this.Config.Extract<AppServerComponentConfig>();
            this.Container.RegisterInstance(unitConfig, ServiceLifetime.Singleton);

            this.Container.Register<InitOnlineMeasurementHandler>(ServiceLifetime.Singleton);
            this.Container.Register<MeasurementDispatcher>(ServiceLifetime.Singleton);
        }

        protected override void OnActivateUnit()
        {
            //var gate = this.Resolver.Resolve<IBusGate>();
            //var busEventObserver = this.Resolver.Resolve<IBusEventObserver>();

            var dispatcher = this.Resolver.Resolve<IMessageDispatcher>(); //gate.CreateDispatcher("SDRN.DeviceServer.OnlineMeas.Dispatcher", busEventObserver);

            var initOnlineMeasHandler = this.Resolver.Resolve<InitOnlineMeasurementHandler>();

            dispatcher.RegistryHandler(initOnlineMeasHandler);

            //dispatcher.Activate();
        }

        protected override void OnDeactivateUnit()
        {
        }
        protected override void OnUninstallUnit()
        {
        }
    }
}
