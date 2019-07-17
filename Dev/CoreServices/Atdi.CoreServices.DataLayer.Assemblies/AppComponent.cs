using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.DataLayer.DataEngines;
using Atdi.Platform.AppComponent;
using Atdi.Platform.DependencyInjection;

namespace Atdi.CoreServices.DataLayer.Assemblies
{
    public sealed class AppComponent : ComponentBase
    {
        public AppComponent()
            : base(
                  name: "AssembliesDataLayerCoreServices", 
                  type: ComponentType.CoreServices, 
                  behavior: ComponentBehavior.Simple | ComponentBehavior.SingleInstance)
        {
        }

        protected override void OnInstall()
        {
            this.Container
                .Register<ServiceObjectResolver>(ServiceLifetime.Singleton);
            this.Container
                .Register<QueryPatternFactory>(ServiceLifetime.Singleton);
            this.Container
                .Register<EngineExecuter>(ServiceLifetime.Singleton);
            this.Container
                .Register<IAssembliesDataEngine, DataEngine>(ServiceLifetime.Transient);
        }
    }
}
