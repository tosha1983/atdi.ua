using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels.EntityOrm;
using Atdi.Platform;
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
	        var componentConfig = this.Config.Extract<EntityOrmComponentConfig>();
	        this.Container.RegisterInstance(componentConfig, ServiceLifetime.Singleton);

	        var defaultEntityOrmConfig = new EntityOrmConfig(componentConfig.DefaultEnvironmentPath);

            this.Container.RegisterInstance<IEntityOrmConfig>(defaultEntityOrmConfig);
            this.Container.Register<DataTypeSystem, DataTypeSystem>(ServiceLifetime.Singleton);
			// теперь это дефолтный ОРМ
            this.Container.Register<IEntityOrm, EntityOrm>(ServiceLifetime.Singleton);
            this.Container.Register<PatternBuilderFactory, PatternBuilderFactory>(ServiceLifetime.Singleton);
            this.Container.Register<IDataLayer<EntityDataOrm>, EntityOrmDataLayer>(ServiceLifetime.PerThread);


        }

        protected override void OnActivate()
        {
	        var typeResolver = this.Resolver.Resolve<ITypeResolver>();
			// сканируем все сборки и ищим тип ынасоедники от EntityOrmContext
			var entityOrmContextType = typeof(EntityOrmContext);
			var entityOrmContextTypes = typeResolver.ForeachInAllAssemblies(
				(type) =>
				{
					if (!type.IsClass
					    || type.IsAbstract
					    || type.IsInterface
					    || type.IsEnum)
					{
						return false;
					}

					return type.BaseType != null && type.BaseType == entityOrmContextType;
				}
			).ToArray();

			foreach (var contextType in entityOrmContextTypes)
			{
				var targetType =
					typeof(IDataLayer<>).MakeGenericType(typeof(EntityDataOrm<>).MakeGenericType(contextType));
				var implementType =
					typeof(EntityOrmDataLayer<>).MakeGenericType(contextType);

				this.Container.Register(targetType, implementType, ServiceLifetime.PerThread);
			}
			base.OnActivate();
        }
    }
}
