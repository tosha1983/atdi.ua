using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppComponent;
using Atdi.DataModels.EntityOrm;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;




namespace Atdi.AppUnits.Sdrn.DeviceServer.Repositories
{
    public sealed class RepositoryComponent : ComponentBase
    {
        public RepositoryComponent()
            : base(
                  name: "RepositoryDeviceServices", 
                  type: ComponentType.CoreServices, 
                  behavior: ComponentBehavior.Simple | ComponentBehavior.SingleInstance)
        {
        }

        protected override void OnInstall()
        {
            var configRepository = this.Config.Extract<ConfigRepositories>();
            this.Container.RegisterInstance(configRepository, Platform.DependencyInjection.ServiceLifetime.Singleton);
            this.Container.Register<IRepository<TaskParameters, string>, TaskParametersByStringRepository>(Platform.DependencyInjection.ServiceLifetime.Singleton);
            this.Container.Register<IRepository<MeasResults, string>, MeasResultsByStringRepository>(Platform.DependencyInjection.ServiceLifetime.Singleton);
            this.Container.Register<IRepository<DeviceCommandResult, string>, DeviceCommandResultByStringRepository>(Platform.DependencyInjection.ServiceLifetime.Singleton);
            this.Container.Register<IRepository<DeviceCommand, string>, SendCommandMeasTaskByStringRepository>(Platform.DependencyInjection.ServiceLifetime.Singleton);
        }
    }
}

