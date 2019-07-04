using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.DataLayer.DataEngines;
using Atdi.Platform.AppComponent;
using Atdi.Platform.DependencyInjection;

namespace Atdi.CoreServices.DataLayer.SqlServer
{
    public sealed class AppComponent : ComponentBase
    {
        public AppComponent()
            : base(
                  name: "SqlServerDataLayerCoreServices", 
                  type: ComponentType.CoreServices, 
                  behavior: ComponentBehavior.Simple | ComponentBehavior.SingleInstance)
        {
        }

        protected override void OnInstall()
        {
            this.Container
                .Register<QueryPatternFactory>(ServiceLifetime.Singleton);
            this.Container
                .Register<EngineSyntax>(ServiceLifetime.Singleton);
            this.Container
                .Register<ISqlServerDataEngine, DataEngine>(ServiceLifetime.Transient);
        }
    }
}
