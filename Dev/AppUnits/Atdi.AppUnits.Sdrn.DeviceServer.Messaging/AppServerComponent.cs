using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            
        }

        protected override void OnActivateUnit()
        {
            var typeResolver = this.Resolver.Resolve<ITypeResolver>();

            
            
        }

        protected override void OnDeactivateUnit()
        {
        }
        protected override void OnUninstallUnit()
        {
        }
    }
}
