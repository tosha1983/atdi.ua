using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Modules.Licensing;
using Atdi.Modules.Sdrn.MessageBus;
using System;
using System.IO;
using System.Reflection;
using Atdi.Modules.Sdrn.DeviceBus;

namespace Atdi.Api.Sdrn.Device.BusController
{
    public sealed class BusGateFactory : IBusGateFactory
    {
        private BusGateFactory()
        {
        }

        public IBusGateConfig CreateConfig()
        {
            return new BusGateConfig();
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

        private string VerifyLicense(string licenseFileName, string ownerId, string productKey)
        {
            if (string.IsNullOrEmpty(licenseFileName))
            {
                throw new ArgumentException("Undefined value", nameof(licenseFileName));
            }

            if (string.IsNullOrEmpty(ownerId))
            {
                throw new ArgumentException("Undefined value", nameof(ownerId));
            }

            if (string.IsNullOrEmpty(productKey))
            {
                throw new ArgumentException("Undefined value", nameof(productKey));
            }

            try
            {
                var verificationData = new VerificationData2
                {
                    OwnerId = ownerId,
                    ProductName = "ICS Control Device",
                    ProductKey = productKey,
                    LicenseType = "DeviceLicense",
                    Date = DateTime.Now,
                    YearHash = LicenseVerifier.EncodeYear(2020)
                };

                licenseFileName = Path.Combine(this.AssemblyDirectory, licenseFileName);
                var licenseBody = File.ReadAllBytes(licenseFileName);

                var verResult = LicenseVerifier.Verify(verificationData, licenseBody);

                return verResult.Instance;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("The license verification failed", e);
            }
        }

        private DeviceBusConfig PrepareEnvironmentDescriptor(IBusGateConfig gateConfig, BusLogger logger)
        {
            var descriptor = new DeviceBusConfig(gateConfig);

            var hasParamError = !this.TryGetConfigParameter(gateConfig, ConfigParams.LicenseFileName, out var licenseFileName, logger);

            if (!this.TryGetConfigParameter(gateConfig, ConfigParams.LicenseOwnerId, out var licenseOwnerId, logger))
            {
                hasParamError = true;
            }

            if (!this.TryGetConfigParameter(gateConfig, ConfigParams.LicenseProductKey, out var licenseProductKey, logger))
            {
                hasParamError = true;
            }

            if (!hasParamError)
            {
                descriptor.SdrnDeviceSensorName = this.VerifyLicense(licenseFileName, licenseOwnerId, licenseProductKey);
            }

            // Rabbit MQ

            if (this.TryGetConfigParameter(gateConfig, ConfigParams.RabbitMQHost, out var paramValue, logger))
            {
                descriptor.RabbitMqHost = paramValue;
            }
            else
            {
                hasParamError = true;
            }

            if (gateConfig.TryGetValue(ConfigParams.RabbitMQPort, out paramValue))
            {
                descriptor.RabbitMqPort = string.IsNullOrEmpty(paramValue) ? (int?)null : Convert.ToInt32(paramValue) ;
            }

            if (this.TryGetConfigParameter(gateConfig, ConfigParams.RabbitMQVirtualHost, out paramValue, logger))
            {
                descriptor.RabbitMqVirtualHost = paramValue;
            }
            else
            {
                descriptor.RabbitMqVirtualHost = "/";
            }

            if (this.TryGetConfigParameter(gateConfig, ConfigParams.RabbitMQUser, out paramValue, logger))
            {
                descriptor.RabbitMqUser = paramValue;
            }
            else
            {
                hasParamError = true;
            }

            if (gateConfig.TryGetValue(ConfigParams.RabbitMQPassword, out paramValue))
            {
                descriptor.RabbitMqPassword = paramValue;
            }


            if (this.TryGetConfigParameter(gateConfig, ConfigParams.SdrnApiVersion, out paramValue, logger))
            {
                descriptor.SdrnApiVersion = paramValue;
            }
            else
            {
                hasParamError = true;
            }

            if (this.TryGetConfigParameter(gateConfig, ConfigParams.SdrnDeviceExchange, out paramValue, logger))
            {
                descriptor.SdrnDeviceExchange = paramValue;
            }
            else
            {
                hasParamError = true;
            }

            if (this.TryGetConfigParameter(gateConfig, ConfigParams.SdrnDeviceMessagesBindings, out paramValue, logger))
            {
                descriptor.SdrnDeviceMessagesBindings = paramValue;
            }
            else
            {
                hasParamError = true;
            }

            if (this.TryGetConfigParameter(gateConfig, ConfigParams.SdrnDeviceQueueNamePart, out paramValue, logger))
            {
                descriptor.SdrnDeviceQueueNamePart = paramValue;
            }
            else
            {
                hasParamError = true;
            }

            if (this.TryGetConfigParameter(gateConfig, ConfigParams.SdrnDeviceSensorTechId, out paramValue, logger))
            {
                descriptor.SdrnDeviceSensorTechId = paramValue;
            }
            else
            {
                hasParamError = true;
            }

            if (this.TryGetConfigParameter(gateConfig, ConfigParams.SdrnServerInstance, out paramValue, logger))
            {
                descriptor.SdrnServerInstance = paramValue;
            }
            else
            {
                hasParamError = true;
            }

            if (this.TryGetConfigParameter(gateConfig, ConfigParams.SdrnServerQueueNamePart, out paramValue, logger))
            {
                descriptor.SdrnServerQueueNamePart = paramValue;
            }
            else
            {
                hasParamError = true;
            }

            if (this.TryGetConfigParameter(gateConfig, ConfigParams.SdrnMessageConvertorUseEncryption, out paramValue, logger))
            {
                descriptor.SdrnMessageConvertorUseEncryption = "true".Equals(paramValue, StringComparison.OrdinalIgnoreCase);
            }
            if (this.TryGetConfigParameter(gateConfig, ConfigParams.SdrnMessageConvertorUseCompression, out paramValue, logger))
            {
                descriptor.SdrnMessageConvertorUseCompression = "true".Equals(paramValue, StringComparison.OrdinalIgnoreCase);
            }

            if (gateConfig.TryGetValue(ConfigParams.DeviceBusOutboxUseBuffer, out paramValue))
            {
                if (BufferType.Filesystem.ToString().Equals(paramValue, StringComparison.OrdinalIgnoreCase))
                {
                    descriptor.OutboxBufferConfig.Type = BufferType.Filesystem;
                   
                    if (this.TryGetConfigParameter(gateConfig, ConfigParams.DeviceBusOutboxBufferContentType, out paramValue, logger))
                    {
                        descriptor.OutboxBufferConfig.ContentType = (ContentType)Enum.Parse(typeof(ContentType), paramValue);
                    }
                    else
                    {
                        hasParamError = true;
                    }
                    if (this.TryGetConfigParameter(gateConfig, ConfigParams.DeviceBusOutboxBufferFolder, out paramValue, logger))
                    {
                        descriptor.OutboxBufferConfig.Folder = paramValue;
                    }
                    else
                    {
                        hasParamError = true;
                    }
                }
                else if (BufferType.Database.ToString().Equals(paramValue, StringComparison.OrdinalIgnoreCase))
                {
                    descriptor.OutboxBufferConfig.Type = BufferType.Database;
                    if (this.TryGetConfigParameter(gateConfig, ConfigParams.DeviceBusOutboxBufferContentType, out paramValue, logger))
                    {
                        descriptor.OutboxBufferConfig.ContentType = (ContentType)Enum.Parse(typeof(ContentType), paramValue);
                    }
                    else
                    {
                        hasParamError = true;
                    }
                    if (this.TryGetConfigParameter(gateConfig, ConfigParams.DeviceBusOutboxBufferConnectionString, out paramValue, logger))
                    {
                        descriptor.OutboxBufferConfig.ConnectionString = paramValue;
                    }
                    else
                    {
                        hasParamError = true;
                    }
                }

            }
            else
            {
                descriptor.OutboxBufferConfig.Type = BufferType.None;
            }

            if (gateConfig.TryGetValue(ConfigParams.DeviceBusInboxUseBuffer, out paramValue))
            {
                if (BufferType.Filesystem.ToString().Equals(paramValue, StringComparison.OrdinalIgnoreCase))
                {
                    descriptor.InboxBufferConfig.Type = BufferType.Filesystem;

                    if (this.TryGetConfigParameter(gateConfig, ConfigParams.DeviceBusInboxBufferContentType, out paramValue, logger))
                    {
                        descriptor.InboxBufferConfig.ContentType = (ContentType)Enum.Parse(typeof(ContentType), paramValue);
                    }
                    else
                    {
                        hasParamError = true;
                    }
                    if (this.TryGetConfigParameter(gateConfig, ConfigParams.DeviceBusInboxBufferFolder, out paramValue, logger))
                    {
                        descriptor.InboxBufferConfig.Folder = paramValue;
                    }
                    else
                    {
                        hasParamError = true;
                    }
                }
                else if (BufferType.Database.ToString().Equals(paramValue, StringComparison.OrdinalIgnoreCase))
                {
                    descriptor.InboxBufferConfig.Type = BufferType.Database;
                    if (this.TryGetConfigParameter(gateConfig, ConfigParams.DeviceBusInboxBufferContentType, out paramValue, logger))
                    {
                        descriptor.InboxBufferConfig.ContentType = (ContentType)Enum.Parse(typeof(ContentType), paramValue);
                    }
                    else
                    {
                        hasParamError = true;
                    }
                    if (this.TryGetConfigParameter(gateConfig, ConfigParams.DeviceBusInboxBufferConnectionString, out paramValue, logger))
                    {
                        descriptor.InboxBufferConfig.ConnectionString = paramValue;
                    }
                    else
                    {
                        hasParamError = true;
                    }
                }

            }
            else
            {
                descriptor.InboxBufferConfig.Type = BufferType.None;
            }
            if (gateConfig.TryGetValue(ConfigParams.DeviceBusContentType, out paramValue))
            {
                descriptor.DeviceBusContentType = (ContentType)Enum.Parse(typeof(ContentType), paramValue);
            }
            else
            {
                descriptor.DeviceBusContentType = ContentType.Sdrn;
            }

            if (gateConfig.TryGetValue(ConfigParams.DeviceBusClient, out paramValue))
            {
                descriptor.DeviceBusClient = paramValue;
            }
            if (gateConfig.TryGetValue(ConfigParams.DeviceBusSharedSecretKey, out paramValue))
            {
                descriptor.DeviceBusSharedSecretKey = paramValue;
            }

            if (hasParamError)
            {
                throw new InvalidOperationException("There are incorrect configuration parameters");
            }
            return descriptor;
        }

        private bool TryGetConfigParameter(IBusGateConfig gateConfig, string paramName, out string result, BusLogger logger)
        {
            result = string.Empty;
            if (!gateConfig.TryGetValue(paramName, out result))
            {
                logger.Error(BusEvents.ConfigParameterError, "DeviceBus.Validation", $"The parameter with name '{paramName}' is undefined", this);
                return false;
            }
            if (string.IsNullOrEmpty(result))
            {
                logger.Error(BusEvents.ConfigParameterError, "DeviceBus.Validation", $"The parameter with name '{paramName}' is empty", this);
                return false;
            }
            return true;
        }
        

        public IBusGate CreateGate(string gateTag, IBusGateConfig gateConfig, IBusEventObserver eventObserver = null)
        {
            try
            {
                var logger = new BusLogger(eventObserver);
                var deviceBusConfig = this.PrepareEnvironmentDescriptor(gateConfig, logger);

                var gate = new BusGate(gateTag, deviceBusConfig, logger);

                logger.Info(0, "DeviceBus.GateCreation", $"The bus gate is created successfully: {gate}, {deviceBusConfig}", this);

                return gate;
            }
            catch(Exception e)
            {
                throw new InvalidOperationException($"The bus gate is not created: Tag='{gateTag}", e);
            }
        }

        public static IBusGateFactory Create()
        {
            return new BusGateFactory();
        }
    }
}
