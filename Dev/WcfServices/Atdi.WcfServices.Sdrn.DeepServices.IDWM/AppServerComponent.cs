using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform;
using Atdi.Platform.AppServer;
using Atdi.Platform.AppComponent;

namespace Atdi.WcfServices.Sdrn.DeepServices.IDWM
{
    public class AppServerComponent : WcfServicesComponent
    {
        public AppServerComponent() : base("SdrnDeepServicesIdwmWcfServices", ComponentBehavior.SingleInstance)
        {

        }

        protected override void OnInstall()
        {
            base.OnInstall();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
	        base.OnDeactivate();
        }

        protected override void OnUninstall()
        {
            base.OnUninstall();
        }
    }
}
