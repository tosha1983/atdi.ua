using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.AppComponent;


namespace Atdi.CoreServices.DataLayer
{
    public sealed class DataLayerComponent : ComponentBase
    {
        public DataLayerComponent()
            : base(
                  name: "DataLayerCoreServices", 
                  type: ComponentType.CoreServices, 
                  behavior: ComponentBehavior.Simple | ComponentBehavior.SingleInstance)
        {
        }

        protected override void OnInstall()
        {
            var dataLayerConfig = new DataLayerConfig(this.Config);

            this.Container.RegisterInstance<IDataLayerConfig>(dataLayerConfig);
            this.Container.Register<IDataLayer, DataLayer>(Platform.DependencyInjection.ServiceLifetime.Singleton);
        }
    }
}
