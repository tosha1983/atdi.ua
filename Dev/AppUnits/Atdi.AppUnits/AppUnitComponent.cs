using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform;
using Atdi.Platform.AppComponent;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits
{
    public class AppUnitComponent : ComponentBase
    {
        public AppUnitComponent(string name) : base(name, ComponentType.AppUnit, ComponentBehavior.SingleInstance)
        {

        }

        public AppUnitComponent(string name, ComponentBehavior behavior) : base(name, ComponentType.AppUnit, behavior)
        {

        }

        protected override void OnInstall()
        {
            base.OnInstall();
            this.OnInstallUnit();
        }
        protected override void OnUninstall()
        {
            base.OnUninstall();
            this.OnUninstallUnit();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            this.OnActivateUnit();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            this.OnDeactivateUnit();
        }

        protected virtual void OnInstallUnit() { }
        protected virtual void OnUninstallUnit() { }
        protected virtual void OnActivateUnit() { }
        protected virtual void OnDeactivateUnit() { }
    }
}
