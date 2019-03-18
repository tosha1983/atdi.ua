using Atdi.Api.Sdrn.Device.BusController;
using Atdi.AppUnits.Sdrn.DeviceServer.Controller;
using Atdi.AppUnits.Sdrn.DeviceServer.Processing;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Modules.Licensing;
using Atdi.Platform;
using Atdi.Platform.AppComponent;
using Atdi.Platform.AppServer;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer
{
    public class AppServerComponent : AppUnitComponent
    {

        public AppServerComponent() 
            : base("SdrnDeviceServerAppUnit")
        {
            
        }

        protected override void OnInstallUnit()
        {
            this.Container.Register<ITimeService, TimeService>(ServiceLifetime.Singleton);

            var gateFactory = BusGateFactory.Create();
            this.Container.RegisterInstance<IBusGateFactory>(gateFactory, ServiceLifetime.Singleton);

            Logger.Verbouse(Contexts.ThisComponent, Categories.Initilazing, Events.GateFactoryWasCreated);

            var gateConfig = this.PrepareGateConfig(gateFactory);
            var deviceServerDevice = this.PrepareDeviceServerConfig(gateConfig);
            this.Container.RegisterInstance<IDeviceServerConfig>(deviceServerDevice, ServiceLifetime.Singleton);

            Logger.Verbouse(Contexts.ThisComponent, Categories.Initilazing, Events.GateConfigWasLoaded.With(gateConfig.GetApi(), gateConfig.GetSdrnServerInstance(), gateConfig.GetRabbitMQHost(), gateConfig.GetRabbitMQPort()));

            var busEventObserver = new BusEventObserver(this.Logger);
            this.Container.RegisterInstance<IBusEventObserver>(busEventObserver, ServiceLifetime.Singleton);

            // фаза регистрации в шине, можем не проходит и важно из-за этого весь процес не остановить, т.е. жить без шины могём
            var gateTag = $"Gate [{deviceServerDevice.SensorName}]";
            try
            {
                var gate = gateFactory.CreateGate(gateTag, gateConfig, busEventObserver);
                this.Container.RegisterInstance<IBusGate>(gate, ServiceLifetime.Singleton);

                Logger.Verbouse(Contexts.ThisComponent, Categories.Initilazing, Events.GateFactoryWasCreated.With(gateTag));
            }
            catch(Exception e)
            {
               this.Logger.Exception(Contexts.ThisComponent, Categories.Initilazing, e);
            }

            this.Container.Register<IEventWaiter, EventWaiter>(ServiceLifetime.Singleton);
            this.Container.Register<IProcessingDispatcher, ProcessingDispatcher>(ServiceLifetime.Singleton);
            this.Container.Register<ITaskWorkerFactory, TaskWorkerFactory>(ServiceLifetime.Singleton);
            this.Container.Register<ITaskWorkersHost, TaskWorkersHost>(ServiceLifetime.Singleton);
            this.Container.Register<IWorkScheduler, WorkScheduler>(ServiceLifetime.Singleton);
            this.Container.Register<ITaskStarter, TaskStarter>(ServiceLifetime.Singleton);
            this.Container.Register<IAutoTaskActivator, AutoTaskActivator>(ServiceLifetime.Singleton);

            Logger.Verbouse(Contexts.ThisComponent, Categories.Initilazing, Events.ProcessingObjectsWereRegistered);

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

            Logger.Verbouse(Contexts.ThisComponent, Categories.Initilazing, Events.ControllerObjectsWereRegistered);
        }

        protected override void OnActivateUnit()
        {
            var typeResolver = this.Resolver.Resolve<ITypeResolver>();

            var workersHost = this.Resolver.Resolve<ITaskWorkersHost>();
            var workerTypes = typeResolver.GetTypesByInterface(typeof(ITaskWorker<,,>));
            foreach (var workerType in workerTypes)
            {
                var lifetime = AppServerComponent.GetTaskWorkerServiceLifetime(workerType);
                this.Container.Register(workerType, workerType, lifetime);
                workersHost.Register(workerType);
            }

            Logger.Info(Contexts.ThisComponent, Categories.Initilazing, Events.TaskWorkerTypesWereRegistered);

            var handlersHost = this.Resolver.Resolve<IResultHandlersHost>();
            var handlerTypes = typeResolver.GetTypesByInterface(typeof(IResultHandler<,,,>));

            foreach (var handlerType in handlerTypes)
            {
                this.Container.Register(handlerType, handlerType, ServiceLifetime.PerThread);
                handlersHost.Register(handlerType);
            }

            Logger.Info(Contexts.ThisComponent, Categories.Initilazing, Events.ResultHandlerTypesWereRegistered);

            var convertorsHost = this.Resolver.Resolve<IResultConvertorsHost>();
            var convertorTypes = typeResolver.GetTypesByInterface(typeof(IResultConvertor<,>));

            foreach (var convertorType in convertorTypes)
            {
                this.Container.Register(convertorType, convertorType, ServiceLifetime.Singleton);
                convertorsHost.Register(convertorType);
            }

            Logger.Info(Contexts.ThisComponent, Categories.Initilazing, Events.ResultConvertorTypesWereRegistered);

            // Scan assemblies loaded into memmory to include the types 
            // that implements the interface "IAdapter" in the container and in the devices host 

            var devicesHost = this.Resolver.Resolve<IDevicesHost>();
            var eventWaiter = this.Resolver.Resolve<IEventWaiter>();
            var adapterTypes = typeResolver.GetTypesByInterface<IAdapter>();

            var adapterRegistrationTimeout = this.Config.GetParameterAsInteger("SDRN.DeviceServer.AdapterRegistrationTimeoutSec") ?? -1;
            if (adapterRegistrationTimeout > 0)
            {
                adapterRegistrationTimeout *= 1000;
            }

            var hostLoader = this.Resolver.Resolve<IServerHostLoader>();

            hostLoader.RegisterTrigger("Adapter Registration", () => 
            {
                foreach (var adapterType in adapterTypes)
                {
                    this.Container.Register(adapterType, adapterType, ServiceLifetime.Transient);
                    devicesHost.Register(adapterType);

                    if (eventWaiter.Wait<AdapterWorker>(out AdapterWorker adapterWorker, adapterRegistrationTimeout))
                    {
                        if (adapterWorker.State == DeviceState.Failure)
                        {
                            Logger.Error(Contexts.ThisComponent, Categories.Initilazing, Events.AdapterRegistrationFailed.With(adapterType));
                        }
                        else if (adapterWorker.State == DeviceState.Aborted)
                        {
                            Logger.Error(Contexts.ThisComponent, Categories.Initilazing, Events.AdapterRegistrationAborted.With(adapterType));
                        }
                        else
                        {
                            Logger.Info(Contexts.ThisComponent, Categories.Initilazing, Events.AdapterRegistrationCompleted.With(adapterType, adapterWorker.DeviceId));
                        }
                    }
                    else
                    {
                        Logger.Error(Contexts.ThisComponent, Categories.Initilazing, Events.AdapterRegistrationTimedOut.With(adapterType));
                    }
                }

                Logger.Info(Contexts.ThisComponent, Categories.Initilazing, Events.AdapterObjectsWereRegistered);
            });

            // Start AutoTasks

            hostLoader.RegisterTrigger("Running auto tasks", () =>
            {
                var autoTaskActivator = this.Resolver.Resolve<IAutoTaskActivator>();
                autoTaskActivator.Run();

                Logger.Info(Contexts.ThisComponent, Categories.Initilazing, Events.AutoTasksWereWtarted);
            });

            
        }

        private string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetAssembly(this.GetType()).CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private VerificationResult LoadLicenseInfo()
        {
            try
            {
                var productKey = this.Config.GetParameterAsDecodeString("License.ProductKey", "Atdi.WcfServices.Sdrn.Device");
                var ownerId = this.Config.GetParameterAsDecodeString("License.OwnerId", "Atdi.WcfServices.Sdrn.Device");

                var verificationData = new VerificationData
                {
                    OwnerId = ownerId,
                    ProductName = "ICS Control Device",
                    ProductKey = productKey,
                    LicenseType = "DeviceLicense",
                    Date = DateTime.Now
                };

                var licenseFileName = this.Config.GetParameterAsString("License.FileName");
                licenseFileName = Path.Combine(this.AssemblyDirectory, licenseFileName);
                var licenseBody = File.ReadAllBytes(licenseFileName);

                var verResult = LicenseVerifier.Verify(verificationData, licenseBody);

                return verResult;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("The license verification failed", e);
            }
        }
        private static ServiceLifetime GetTaskWorkerServiceLifetime(Type workerInstanceType)
        {
            var interfaceType = workerInstanceType.GetInterface(typeof(ITaskWorker<,,>).Name);
            var lifetimeType = interfaceType.GenericTypeArguments[2];
            if (lifetimeType == typeof(SingletonTaskWorkerLifetime))
            {
                return ServiceLifetime.Singleton;
            }
            if (lifetimeType == typeof(PerThreadTaskWorkerLifetime))
            {
                return ServiceLifetime.PerThread;
            }
            if (lifetimeType == typeof(TransientTaskWorkerLifetime))
            {
                return ServiceLifetime.Transient;
            }

            throw new InvalidOperationException($"Unsupported the type of life time: {lifetimeType}");
        }

        private IDeviceServerConfig PrepareDeviceServerConfig(IBusGateConfig busGateConfig)
        {
            var licenseInfo = this.LoadLicenseInfo();
            var config = new DeviceServerConfig()
            {
                SdrnServerInstance = busGateConfig.GetSdrnServerInstance(),
                SensorTechId = busGateConfig.GetValue<string>(ConfigParams.SdrnDeviceSensorTechId),
                SensorName = licenseInfo.Instance,
                LicenseNumber = licenseInfo.LicenseNumber,
                LicenseStartDate = licenseInfo.StartDate,
                LicenseStopDate = licenseInfo.StopDate
            };
            
            return config;
        }
        private IBusGateConfig PrepareGateConfig(IBusGateFactory gateFactory)
        {
            var gateConfig = gateFactory.CreateConfig();

            gateConfig[ConfigParams.LicenseFileName] = this.Config.GetParameterAsString(ConfigParams.LicenseFileName);
            gateConfig[ConfigParams.LicenseOwnerId] = this.Config.GetParameterAsDecodeString(ConfigParams.LicenseOwnerId, "Atdi.WcfServices.Sdrn.Device");
            gateConfig[ConfigParams.LicenseProductKey] = this.Config.GetParameterAsDecodeString(ConfigParams.LicenseProductKey, "Atdi.WcfServices.Sdrn.Device");

            gateConfig[ConfigParams.RabbitMQHost] = this.Config.GetParameterAsString(ConfigParams.RabbitMQHost);
            gateConfig[ConfigParams.RabbitMQPort] = this.Config.GetParameterAsString(ConfigParams.RabbitMQPort);
            gateConfig[ConfigParams.RabbitMQVirtualHost] = this.Config.GetParameterAsString(ConfigParams.RabbitMQVirtualHost);
            gateConfig[ConfigParams.RabbitMQUser] = this.Config.GetParameterAsString(ConfigParams.RabbitMQUser);
            gateConfig[ConfigParams.RabbitMQPassword] = this.Config.GetParameterAsDecodeString(ConfigParams.RabbitMQPassword, "Atdi.WcfServices.Sdrn.Device");

            gateConfig[ConfigParams.SdrnApiVersion] = this.Config.GetParameterAsString(ConfigParams.SdrnApiVersion);

            gateConfig[ConfigParams.SdrnServerInstance] = this.Config.GetParameterAsString(ConfigParams.SdrnServerInstance);
            gateConfig[ConfigParams.SdrnServerQueueNamePart] = this.Config.GetParameterAsString(ConfigParams.SdrnServerQueueNamePart);

            gateConfig[ConfigParams.SdrnDeviceSensorTechId] = this.Config.GetParameterAsString(ConfigParams.SdrnDeviceSensorTechId);
            gateConfig[ConfigParams.SdrnDeviceExchange] = this.Config.GetParameterAsString(ConfigParams.SdrnDeviceExchange);
            gateConfig[ConfigParams.SdrnDeviceQueueNamePart] = this.Config.GetParameterAsString(ConfigParams.SdrnDeviceQueueNamePart);
            gateConfig[ConfigParams.SdrnDeviceMessagesBindings] = this.Config.GetParameterAsString(ConfigParams.SdrnDeviceMessagesBindings);
            gateConfig[ConfigParams.SdrnMessageConvertorUseEncryption] = this.Config.GetParameterAsString(ConfigParams.SdrnMessageConvertorUseEncryption);
            gateConfig[ConfigParams.SdrnMessageConvertorUseCompression] = this.Config.GetParameterAsString(ConfigParams.SdrnMessageConvertorUseCompression);

            return gateConfig;
        }

        protected override void OnDeactivateUnit()
        {
        }
        protected override void OnUninstallUnit()
        {
        }
    }
}
