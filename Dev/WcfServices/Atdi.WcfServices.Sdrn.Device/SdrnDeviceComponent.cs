using System;
using System.IO;
using System.Reflection;
using Atdi.Api.Sdrn.Device.BusController;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Modules.Licensing;
using Atdi.Platform.AppComponent;
using Atdi.Platform.Logging;
using Atdi.Platform.DependencyInjection;
using Atdi.Contracts.WcfServices.Sdrn.Device;
using Atdi.Platform.AppServer;

namespace Atdi.WcfServices.Sdrn.Device
{
    public class SdrnDeviceComponent : WcfServicesComponent
    {
        private SdrnServerDescriptor _serverDescriptor;

        public SdrnDeviceComponent() : base("SdrnDeviceServices", ComponentBehavior.SingleInstance)
        {
           
        }

        private string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetAssembly(this.GetType()).CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        protected override void OnInstall()
        {
            base.OnInstall();

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

                this._serverDescriptor = new SdrnServerDescriptor(this.Config, verResult.Instance);
                var serviceEnvironment = new ServiceEnvironment(this._serverDescriptor, verResult);

                this.Container.RegisterInstance(this._serverDescriptor, ServiceLifetime.Singleton);
                this.Container.RegisterInstance<IServiceEnvironment>(serviceEnvironment, ServiceLifetime.Singleton);
            }
            catch(Exception e)
            {
                throw new InvalidOperationException("The license verification failed", e);
            }

            var gateFactory = BusGateFactory.Create();
            this.Container.RegisterInstance<IBusGateFactory>(gateFactory, ServiceLifetime.Singleton);

            Logger.Info(Contexts.ThisComponent, Categories.Initialization, Events.GateFactoryWasCreated);

            var gateConfig = this.PrepareGateConfig(gateFactory);
            Logger.Info(Contexts.ThisComponent, Categories.Initialization, Events.GateConfigWasLoaded.With(gateConfig.GetApi(), gateConfig.GetSdrnServerInstance(), gateConfig.GetRabbitMQHost(), gateConfig.GetRabbitMQPort()));

            var busEventObserver = new BusEventObserver(this.Logger);
            this.Container.RegisterInstance<IBusEventObserver>(busEventObserver, ServiceLifetime.Singleton);

            // фаза регистрации в шине, можем не проходит и важно из-за этого весь процес не остановить, т.е. жить без шины могём
            var gateTag = $"Device WCF Service [{_serverDescriptor.SensorName}]";
            try
            {
                var gate = gateFactory.CreateGate(gateTag, gateConfig, busEventObserver);
                this.Container.RegisterInstance<IBusGate>(gate, ServiceLifetime.Singleton);

                Logger.Info(Contexts.ThisComponent, Categories.Initialization, Events.GateWasCreated.With(gateTag));
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.ThisComponent, Categories.Initialization, e);
            }
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
            gateConfig[ConfigParams.DeviceBusContentType] = this.Config.GetParameterAsString(ConfigParams.DeviceBusContentType);

            gateConfig[ConfigParams.DeviceBusOutboxBufferContentType] = this.Config.GetParameterAsString(ConfigParams.DeviceBusOutboxBufferContentType);
            gateConfig[ConfigParams.DeviceBusOutboxBufferFolder] = this.Config.GetParameterAsString(ConfigParams.DeviceBusOutboxBufferFolder);
            gateConfig[ConfigParams.DeviceBusOutboxUseBuffer] = this.Config.GetParameterAsString(ConfigParams.DeviceBusOutboxUseBuffer);

            gateConfig[ConfigParams.DeviceBusInboxBufferContentType] = this.Config.GetParameterAsString(ConfigParams.DeviceBusInboxBufferContentType);
            gateConfig[ConfigParams.DeviceBusInboxBufferFolder] = this.Config.GetParameterAsString(ConfigParams.DeviceBusInboxBufferFolder);
            gateConfig[ConfigParams.DeviceBusInboxUseBuffer] = this.Config.GetParameterAsString(ConfigParams.DeviceBusInboxUseBuffer);

            gateConfig[ConfigParams.DeviceBusSharedSecretKey] = this.Config.GetParameterAsString(ConfigParams.DeviceBusSharedSecretKey);

            gateConfig[ConfigParams.DeviceBusClient] =
                "DeviceWcfService: " + Assembly.GetAssembly(this.GetType()).GetName().Version;

            return gateConfig;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            var gate = this.Resolver.Resolve<IBusGate>();
            var busEventObserver = this.Resolver.Resolve<IBusEventObserver>();
            var dispatcher = gate.CreateDispatcher("SDRN.DeviceWcfService", busEventObserver);
            var publisher = gate.CreatePublisher("SDRN.DeviceWcfService", busEventObserver);
            this.Container.RegisterInstance<IMessageDispatcher>(dispatcher, ServiceLifetime.Singleton);
            this.Container.RegisterInstance<IMessagePublisher>(publisher, ServiceLifetime.Singleton);
            var hostLoader = this.Resolver.Resolve<IServerHostLoader>();
            hostLoader.RegisterTrigger("Running device bus handlers activation", () =>
            {
                dispatcher.Activate();
            });
        }

        protected override void OnUninstall()
        {
            this._serverDescriptor = null;
            base.OnUninstall();
        }
    }
}
