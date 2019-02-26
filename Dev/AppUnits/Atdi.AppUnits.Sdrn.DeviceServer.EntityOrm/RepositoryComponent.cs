using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.AppComponent;
using Atdi.DataModels.EntityOrm;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Contracts.CoreServices.EntityOrm;



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
            this.Container.Register<IRepository<Sensor, int?>, SensorsRepository>(Platform.DependencyInjection.ServiceLifetime.Singleton);
            this.Container.Register<IRepository<MeasTask, int?>, MeasTaskRepository>(Platform.DependencyInjection.ServiceLifetime.Singleton);
            this.Container.Register<IRepository<TaskParameters, int?>, TaskParametersRepository> (Platform.DependencyInjection.ServiceLifetime.Singleton);
        }
    }
}

