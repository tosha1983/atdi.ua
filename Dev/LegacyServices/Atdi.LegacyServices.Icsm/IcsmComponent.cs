using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.Platform.AppComponent;


namespace Atdi.LegacyServices.Icsm
{
    public sealed class IcsmComponent : ComponentBase
    {
        public IcsmComponent()
            : base(
                  name: "IcsmLegacyServices", 
                  type: ComponentType.LegacyServices, 
                  behavior: ComponentBehavior.Simple | ComponentBehavior.SingleInstance)
        {
        }

        public static string IcsmSchemaPath { get; private set; }
        protected override void OnInstall()
        {
            IcsmComponent.IcsmSchemaPath = Convert.ToString(this.Config["IcsmSchemaPath"]);
            this.Container.Register<IDataLayer<IcsmDataOrm>, IcsmDataLayer>(Platform.DependencyInjection.ServiceLifetime.PerThread);
        }
    }
}
