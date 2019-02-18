using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    public class AppServerComponent : AppUnitComponent
    {

        public AppServerComponent() 
            : base("SdrnDeviceServerControllerAppUnit")
        {
            
        }

        protected override void OnInstallUnit()
        {
            this.Container.Register<IAdapterFactory, AdapterFactory>(ServiceLifetime.Singleton);
            this.Container.Register<IResultHandlerFactory, ResultHandlerFactory>(ServiceLifetime.Singleton);
            this.Container.Register<IResultConvertorFactory, ResultConvertorFactory>(ServiceLifetime.Singleton);
            this.Container.Register<IResultHandlersHost, ResultHandlersHost>(ServiceLifetime.Singleton);
            this.Container.Register<IResultConvertorsHost, ResultConvertorsHost>(ServiceLifetime.Singleton);
            this.Container.Register<IResultsHost, ResultsHost>(ServiceLifetime.Singleton);
            this.Container.Register<ICommandsHost, CommandsHost>(ServiceLifetime.Singleton);
            this.Container.Register<IDevicesHost, DevicesHost>(ServiceLifetime.Singleton);
            this.Container.Register<IDeviceSelector, DeviceSelector>(ServiceLifetime.Singleton);
            this.Container.Register<IController, DevicesController>(ServiceLifetime.Singleton);
        }

        protected override void OnActivateUnit()
        {
            var typeResolver = this.Resolver.Resolve<ITypeResolver>();

            var handlersHost = this.Resolver.Resolve<IResultHandlersHost>();
            var handlerTypes = typeResolver.GetTypesByInterface(typeof(IResultHandler<,,,>));

            foreach (var handlerType in handlerTypes)
            {
                this.Container.Register(handlerType, handlerType, ServiceLifetime.PerThread);
                handlersHost.Register(handlerType);
            }

            var convertorsHost = this.Resolver.Resolve<IResultConvertorsHost>();
            var convertorTypes = typeResolver.GetTypesByInterface(typeof(IResultConvertor<,>));

            foreach (var convertorType in convertorTypes)
            {
                this.Container.Register(convertorType, convertorType, ServiceLifetime.Singleton);
                convertorsHost.Register(convertorType);
            }
        }

        protected override void OnDeactivateUnit()
        {
        }
        protected override void OnUninstallUnit()
        {
        }
    }
}
