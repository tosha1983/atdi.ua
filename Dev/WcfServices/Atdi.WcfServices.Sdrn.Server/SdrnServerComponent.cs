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
using Atdi.Platform.Logging;
using Atdi.Contracts.Sdrn.Server;
using Atdi.Platform.DependencyInjection;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;

namespace Atdi.WcfServices.Sdrn.Server
{
    public class SdrnServerComponent : WcfServicesComponent
    {
        public SdrnServerComponent() : base("SdrnServerWcfServices", ComponentBehavior.SingleInstance)
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

        protected override void OnUninstall()
        {
            base.OnUninstall();
        }
    }
}
