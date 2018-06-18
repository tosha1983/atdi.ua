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
            var schemasPath = Convert.ToString(this.Config[Parameters.SchemasPath]);
            var edition = Convert.ToString(this.Config[Parameters.Edition]);
            var schemasDesc = Convert.ToString(this.Config[Parameters.Schemas]);
            var modulesDesc = Convert.ToString(this.Config[Parameters.Modules]);

            if (string.IsNullOrEmpty(schemasPath))
            {
                throw new ArgumentNullException(Exceptions.UndefinedParameter.With(Parameters.SchemasPath));
            }
            if (string.IsNullOrEmpty(edition))
            {
                throw new ArgumentNullException(Exceptions.UndefinedParameter.With(Parameters.Edition));
            }
            if (string.IsNullOrEmpty(schemasDesc))
            {
                throw new ArgumentNullException(Exceptions.UndefinedParameter.With(Parameters.Schemas));
            }
            if (string.IsNullOrEmpty(modulesDesc))
            {
                throw new ArgumentNullException(Exceptions.UndefinedParameter.With(Parameters.Modules));
            }

            var schemas = schemasDesc.Split(new string[] { ", ", "; ", ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (schemas.Length == 0)
            {
                throw new ArgumentException(Exceptions.UndefinedParameter.With(Parameters.Schemas));
            }

            var modules = modulesDesc.Split(new string[] { ", ", "; ", ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (modules.Length == 0)
            {
                throw new ArgumentException(Exceptions.UndefinedParameter.With(Parameters.Modules));
            }
            var schemasMetadata = new Orm.SchemasMetadata(schemasPath, schemas, modules, edition);

            this.Container.RegisterInstance(typeof(Orm.SchemasMetadata), schemasMetadata, Platform.DependencyInjection.ServiceLifetime.Singleton);
            this.Container.Register<IDataLayer<IcsmDataOrm>, IcsmDataLayer>(Platform.DependencyInjection.ServiceLifetime.PerThread);
            this.Container.Register<IIrpParser, IrpParser>(Platform.DependencyInjection.ServiceLifetime.Singleton);
        }
    }
}
