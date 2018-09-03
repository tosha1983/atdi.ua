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

        
        protected override void OnInstall()
        {
           
            var schemasMetadataConfig = new Orm.SchemasMetadataConfig(this.Config);

            this.Container.RegisterInstance(typeof(Orm.SchemasMetadataConfig), schemasMetadataConfig, Platform.DependencyInjection.ServiceLifetime.Singleton);
            this.Container.Register<Orm.SchemasMetadata>(Platform.DependencyInjection.ServiceLifetime.Singleton);
            this.Container.Register<IDataLayer<IcsmDataOrm>, IcsmDataLayer>(Platform.DependencyInjection.ServiceLifetime.PerThread);
            this.Container.Register<IIrpParser, IrpParser>(Platform.DependencyInjection.ServiceLifetime.Singleton);
        }
    }
}
