using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Modules.Licensing;
using Atdi.Modules.Sdrn.MessageBus;

namespace Atdi.Api.Sdrn.Device.BusController
{
    public sealed class BusGateFactory : IBusGateFactory
    {
        internal BusGateFactory()
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
                string codeBase = Assembly.GetAssembly(this.GetType()).CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private string VerifyLicense(string licenseFileName, string ownerId, string productKey)
        {
            if (string.IsNullOrEmpty(licenseFileName))
            {
                throw new ArgumentException("message", nameof(licenseFileName));
            }

            if (string.IsNullOrEmpty(ownerId))
            {
                throw new ArgumentException("message", nameof(ownerId));
            }

            if (string.IsNullOrEmpty(productKey))
            {
                throw new ArgumentException("message", nameof(productKey));
            }

            try
            {
                var verificationData = new VerificationData
                {
                    OwnerId = ownerId,
                    ProductName = "ICS Control Device",
                    ProductKey = productKey,
                    LicenseType = "DeviceLicense",
                    Date = DateTime.Now
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

        private EnvironmentDescriptor PrepareEnvironmentDescriptor(IBusGateConfig gateConfig, BusLogger logger)
        {
            var descriptor = new EnvironmentDescriptor(gateConfig);
            var hasParamError = false;

            var licenseFileName = string.Empty;
            if (!this.TryGetyConfigParameter(gateConfig, ConfigParams.LicenseFileName, out licenseFileName, logger))
            {
                hasParamError = true;
            }
            var licenseOwnerId = string.Empty;
            if (!this.TryGetyConfigParameter(gateConfig, ConfigParams.LicenseOwnerId, out licenseOwnerId, logger))
            {
                hasParamError = true;
            }
            var licensePoductKey = string.Empty;
            if (!this.TryGetyConfigParameter(gateConfig, ConfigParams.LicenseProductKey, out licensePoductKey, logger))
            {
                hasParamError = true;
            }

            if (!hasParamError)
            {
                descriptor.SdrnDeviceSensorName = this.VerifyLicense(licenseFileName, licenseOwnerId, licensePoductKey);
            }

            var paramValue = string.Empty;

            // Rabbit MQ

            if (this.TryGetyConfigParameter(gateConfig, ConfigParams.RabbitMQHost, out paramValue, logger))
            {
                descriptor.RabbitMQHost = paramValue;
            }
            else
            {
                hasParamError = true;
            }

            if (this.TryGetyConfigParameter(gateConfig, ConfigParams.RabbitMQUser, out paramValue, logger))
            {
                descriptor.RabbitMQUser = paramValue;
            }
            else
            {
                hasParamError = true;
            }

            if (gateConfig.TryGetValue(ConfigParams.RabbitMQPassword, out paramValue))
            {
                descriptor.RabbitMQPassword = paramValue;
            }


            if (this.TryGetyConfigParameter(gateConfig, ConfigParams.SdrnApiVersion, out paramValue, logger))
            {
                descriptor.SdrnApiVersion = paramValue;
            }
            else
            {
                hasParamError = true;
            }

            if (this.TryGetyConfigParameter(gateConfig, ConfigParams.SdrnDeviceExchange, out paramValue, logger))
            {
                descriptor.SdrnDeviceExchange = paramValue;
            }
            else
            {
                hasParamError = true;
            }

            if (this.TryGetyConfigParameter(gateConfig, ConfigParams.SdrnDeviceMessagesBindings, out paramValue, logger))
            {
                descriptor.SdrnDeviceMessagesBindings = paramValue;
            }
            else
            {
                hasParamError = true;
            }

            if (this.TryGetyConfigParameter(gateConfig, ConfigParams.SdrnDeviceQueueNamePart, out paramValue, logger))
            {
                descriptor.SdrnDeviceQueueNamePart = paramValue;
            }
            else
            {
                hasParamError = true;
            }

            if (this.TryGetyConfigParameter(gateConfig, ConfigParams.SdrnDeviceSensorTechId, out paramValue, logger))
            {
                descriptor.SdrnDeviceSensorTechId = paramValue;
            }
            else
            {
                hasParamError = true;
            }

            if (this.TryGetyConfigParameter(gateConfig, ConfigParams.SdrnServerInstance, out paramValue, logger))
            {
                descriptor.SdrnServerInstance = paramValue;
            }
            else
            {
                hasParamError = true;
            }

            if (this.TryGetyConfigParameter(gateConfig, ConfigParams.SdrnServerQueueNamePart, out paramValue, logger))
            {
                descriptor.SdrnServerQueueNamePart = paramValue;
            }
            else
            {
                hasParamError = true;
            }

            if (this.TryGetyConfigParameter(gateConfig, ConfigParams.SdrnMessageConvertorUseEncryption, out paramValue, logger))
            {
                descriptor.SdrnMessageConvertorUseEncryption = "true".Equals(paramValue, StringComparison.OrdinalIgnoreCase);
            }
            if (this.TryGetyConfigParameter(gateConfig, ConfigParams.SdrnMessageConvertorUseСompression, out paramValue, logger))
            {
                descriptor.SdrnMessageConvertorUseСompression = "true".Equals(paramValue, StringComparison.OrdinalIgnoreCase);
            }

            if (hasParamError)
            {
                throw new InvalidOperationException("There are incorrect configuration parameters");
            }
            return descriptor;
        }

        private bool TryGetyConfigParameter(IBusGateConfig gateConfig, string paramName, out string result, BusLogger logger)
        {
            result = string.Empty;
            if (!gateConfig.TryGetValue(paramName, out result))
            {
                logger.Error(BusEvents.ConfigParameterError, " ValidateParameter", $"Parameter with name '{paramName}' was undefined", this);
                return false;
            }
            if (string.IsNullOrEmpty(result))
            {
                logger.Error(BusEvents.ConfigParameterError, " ValidateParameter", $"Parameter with name '{paramName}' is empty", this);
                return false;
            }
            return true;
        }
        private void DeclareRabbitMQEnvironment(EnvironmentDescriptor descriptor, BusLogger logger)
        {
            using (var rabbitBus = new RabbitMQBus("Declaring", descriptor, logger))
            {
                logger.Verbouse("Rabbit MQ: Declaring", $"The connection to Rabbit MQ Server was checked successfully: Host = '{descriptor.RabbitMQHost}'", this);

                var deviceExchangeName = descriptor.BuildDeviceExchangeName();
                rabbitBus.DeclareDurableDirectExchange(deviceExchangeName);

                var bindings = descriptor.MessagesBindings.Values;
                foreach (var binding in bindings)
                {
                    var queueDescriptor = new QueueDescriptor
                    {
                        Exchange = deviceExchangeName,
                        Name = descriptor.BuildServerQueueName(binding.RoutingKey),
                        RoutingKey = $"[{descriptor.SdrnServerInstance}].[{binding.RoutingKey}]"
                    };

                    rabbitBus.DeclareDurableQueue(queueDescriptor);
                }

                var trashQueueDescriptor = new QueueDescriptor
                {
                    Exchange = deviceExchangeName,
                    Name = descriptor.BuildServerQueueName("trash"),
                    RoutingKey = $"[{descriptor.SdrnServerInstance}].[trash]"
                };

                rabbitBus.DeclareDurableQueue(trashQueueDescriptor);

                var errorsQueueDescriptor = new QueueDescriptor
                {
                    Exchange = deviceExchangeName,
                    Name = descriptor.BuildServerQueueName("errors"),
                    RoutingKey = $"[{descriptor.SdrnServerInstance}].[errors]"
                };

                rabbitBus.DeclareDurableQueue(errorsQueueDescriptor);

                var deviceQueueName = descriptor.BuildDeviceQueueName();
                rabbitBus.DeclareDurableQueue(deviceQueueName);
            }
        }

        public IBusGate CreateGate(string gateTag, IBusGateConfig gateConfig, IBusEventObserver eventObserver = null)
        {
            try
            {
                var logger = new BusLogger(eventObserver);
                var descriptor = this.PrepareEnvironmentDescriptor(gateConfig, logger);
                this.DeclareRabbitMQEnvironment(descriptor, logger);

                var convertorSettings = new MessageConvertSettings
                {
                    UseEncryption = descriptor.SdrnMessageConvertorUseEncryption,
                    UseСompression = descriptor.SdrnMessageConvertorUseСompression
                };
                var typeResolver = MessageObjectTypeResolver.CreateForApi20();
                var messageConvertor = new MessageConverter(convertorSettings, typeResolver);

                var gate = new BusGate(gateTag, descriptor, messageConvertor, logger);

                logger.Info(0, "CreateGate", "The object of the gate was created saccessfully", this);

                return gate;
            }
            catch(Exception e)
            {
                throw new InvalidOperationException("The object of the gate was not created", e);
            }
        }

        public static IBusGateFactory Create()
        {
            return new BusGateFactory();
        }
    }
}
