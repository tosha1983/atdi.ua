using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.DataLayer.DataEngines;
using Atdi.Platform.AppComponent;


namespace Atdi.CoreServices.DataLayer.Oracle
{
    public sealed class AppComponent : ComponentBase
    {
        public AppComponent()
            : base(
                  name: "OracleDataLayerCoreServices", 
                  type: ComponentType.CoreServices, 
                  behavior: ComponentBehavior.Simple | ComponentBehavior.SingleInstance)
        {
        }

        protected override void OnInstall()
        {
            this.Container
              .Register<OracleQueryPatternFactory>(Platform.DependencyInjection.ServiceLifetime.Singleton);
            this.Container
                .Register<OracleEngineSyntax>(Platform.DependencyInjection.ServiceLifetime.Singleton);
            this.Container
                .Register<IOracleDataEngine, OracleDataEngine>(Platform.DependencyInjection.ServiceLifetime.Transient);
        }
    }
}
