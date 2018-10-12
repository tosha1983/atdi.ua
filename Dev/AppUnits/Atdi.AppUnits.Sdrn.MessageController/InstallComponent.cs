using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.AppComponent;
using Atdi.Platform.DependencyInjection;
using Atdi.Contracts.Api.Sdrn.MessageBus;

namespace Atdi.AppUnits.Sdrn.MessageController
{
    public sealed class RegisterSensorFromDeviceHandlerComponent : ComponentBase
    {
        public RegisterSensorFromDeviceHandlerComponent()
            : base(
                  name: "RegisterSensorFromDeviceHandler", 
                  type: ComponentType.AppUnit, 
                  behavior: ComponentBehavior.Simple | ComponentBehavior.SingleInstance)
        {
        }

        protected override void OnInstall()
        {
            this.Container.Register<RegisterSensorFromDeviceHandler>(Platform.DependencyInjection.ServiceLifetime.Singleton);
            //this.Container.RegisterInstance<RegisterSensorFromDeviceHandler>(new RegisterSensorFromDeviceHandler(), Platform.DependencyInjection.ServiceLifetime.Singleton);
        }
    }
}
