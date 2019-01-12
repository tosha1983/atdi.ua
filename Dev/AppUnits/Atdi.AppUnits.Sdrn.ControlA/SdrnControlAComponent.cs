using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppUnits.Sdrn.ControlA.Handlers;

namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class SdrnControlAComponent : AppUnitComponent
    {

        public SdrnControlAComponent() 
            : base("SdrnControlAAppUnit")
        {
            
        }



        protected override void OnInstallUnit()
        {
            var busManager = new BusManager(this.Logger, this.Config);
            this.Container.RegisterInstance(typeof(BusManager), busManager, Platform.DependencyInjection.ServiceLifetime.Singleton);
        }

        protected override void OnActivateUnit()
        {
        }

        protected override void OnDeactivateUnit()
        {
        }
        protected override void OnUninstallUnit()
        {
        }
    }
}
