using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public class AppServerComponent : AppUnitComponent
    {

        public AppServerComponent() 
            : base("SdrnDeviceServerProcessingAppUnit")
        {
            
        }

        protected override void OnInstallUnit()
        {
            this.Container.Register<ITaskWorkerFactory, TaskWorkerFactory>(ServiceLifetime.Singleton);
            this.Container.Register<ITaskWorkersHost, TaskWorkersHost>(ServiceLifetime.Singleton);
            this.Container.Register<IProcessingDispatcher, ProcessingDispatcher>(ServiceLifetime.Singleton);
            this.Container.Register<IAutoTaskActivator, AutoTaskActivator>(ServiceLifetime.Singleton);
            
        }

        protected override void OnActivateUnit()
        {
            var typeResolver = this.Resolver.Resolve<ITypeResolver>();

            var workersHost = this.Resolver.Resolve<ITaskWorkersHost>();
            var workerTypes = typeResolver.GetTypesByInterface(typeof(ITaskWorker<,,>));
            foreach (var workerType in workerTypes)
            {

                this.Container.Register(workerType, workerType, ServiceLifetime.Transient);
                workersHost.Register(workerType);
            }

            // Scan assemblies loaded into memmory to include the types 
            // that implements the interface "IAdapter" in the container and in the devices host 

            var devicesHost = this.Resolver.Resolve<IDevicesHost>();
            var adapterTypes = typeResolver.GetTypesByInterface<IAdapter>();
            foreach (var adapterType in adapterTypes)
            {
                this.Container.Register(adapterType, adapterType, ServiceLifetime.Transient);
                devicesHost.Register(adapterType);
            }

            // Start AutoTasks

            var autoTaskActivator = this.Resolver.Resolve<IAutoTaskActivator>();
            var task = autoTaskActivator.Run();
            task.ContinueWith(t =>
            {
                //Logger.Verbouse()
            });
            
        }

        protected override void OnDeactivateUnit()
        {
        }
        protected override void OnUninstallUnit()
        {
        }
    }
}
