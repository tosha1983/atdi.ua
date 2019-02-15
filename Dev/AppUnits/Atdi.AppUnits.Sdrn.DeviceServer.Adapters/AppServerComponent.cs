using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters
{
    public class AppServerComponent : AppUnitComponent
    {

        public AppServerComponent() 
            : base("SdrnDeviceServerAdaptersAppUnit")
        {
            
        }

        protected override void OnInstallUnit()
        {
        }

        protected override void OnActivateUnit()
        {
            // Scan assemblies loaded into memmory to include the types 
            // that implements the interface "IAdapter" in the container and in the devices host 

            var devicesHost = this.Resolver.Resolve<IDevicesHost>();
            var typeResolver = this.Resolver.Resolve<ITypeResolver>();
            var adapterTypes = typeResolver.GetTypesByInterface<IAdapter>();
                
            foreach (var adapterType in adapterTypes)
            {
                this.Container.Register(adapterType, adapterType, ServiceLifetime.Transient);
                devicesHost.Register(adapterType);
            }



        }

        protected override void OnDeactivateUnit()
        {
        }
        protected override void OnUninstallUnit()
        {
        }
    }
}
