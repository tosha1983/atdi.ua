using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.AppComponent;
using Atdi.Platform.DependencyInjection;

namespace Atdi.CoreServices.EntityOrm
{
    public sealed class EntityOrmComponent : ComponentBase
    {
        public EntityOrmComponent()
            : base(
                  name: "EntityOrmCoreServices", 
                  type: ComponentType.CoreServices, 
                  behavior: ComponentBehavior.Simple | ComponentBehavior.SingleInstance)
        {
        }

        protected override void OnInstall()
        {
            var entityOrmConfig = new EntityOrmConfig(this.Config);
            this.Container.RegisterInstance<IEntityOrmConfig>(entityOrmConfig);
            this.Container.Register<DataTypeSystem, DataTypeSystem>(ServiceLifetime.Singleton);
            this.Container.Register<IEntityOrm, EntityOrm>(ServiceLifetime.Singleton);
            this.Container.Register<PatternBuilderFactory, PatternBuilderFactory>(ServiceLifetime.Singleton);
            this.Container.Register<IDataLayer<EntityDataOrm>, EntityOrmDataLayer>(ServiceLifetime.PerThread);
        }
    }
}
