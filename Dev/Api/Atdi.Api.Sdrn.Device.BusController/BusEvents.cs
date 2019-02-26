using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.Sdrn.Device.BusController
{
    public static class BusEvents
    {
        public static readonly int VebouseEvent = 1;
        public static readonly int ExceptionEvent = 1;
        public static readonly int ConfigParameterError = 2;
        public static readonly int NotEstablishConnectionToRabbit = 3;
        public static readonly int NotEstablishRabbitSharedChannel= 4;
        public static readonly int NotDeclareExchange = 5;
        public static readonly int NotCreateConsumer = 6;


    }

    public static class ConfigParams
    {
        public static readonly string LicenseFileName = "License.FileName";
        public static readonly string LicenseOwnerId = "License.OwnerId";
        public static readonly string LicenseProductKey = "License.ProductKey";

        public static readonly string RabbitMQHost = "RabbitMQ.Host";
        public static readonly string RabbitMQPort = "RabbitMQ.Port";
        public static readonly string RabbitMQVirtualHost = "RabbitMQ.VirtualHost";
        public static readonly string RabbitMQUser = "RabbitMQ.User";
        public static readonly string RabbitMQPassword = "RabbitMQ.Password";

        public static readonly string SdrnApiVersion = "SDRN.ApiVersion";
        public static readonly string SdrnServerInstance = "SDRN.Server.Instance";
        public static readonly string SdrnServerQueueNamePart = "SDRN.Server.QueueNamePart";

        public static readonly string SdrnDeviceSensorTechId = "SDRN.Device.SensorTechId";
        public static readonly string SdrnDeviceExchange = "SDRN.Device.Exchange";
        public static readonly string SdrnDeviceQueueNamePart = "SDRN.Device.QueueNamePart";
        public static readonly string SdrnDeviceMessagesBindings = "SDRN.Device.MessagesBindings";

        public static readonly string SdrnMessageConvertorUseEncryption = "SDRN.MessageConvertor.UseEncryption";
        public static readonly string SdrnMessageConvertorUseCompression = "SDRN.MessageConvertor.UseCompression";
    }

    public static class BusGateConfigExtensions
    {
        public static string GetApi(this IBusGateConfig config)
        {
            return config.GetValue<string>(ConfigParams.SdrnApiVersion);
        }
        public static string GetSdrnServerInstance(this IBusGateConfig config)
        {
            return config.GetValue<string>(ConfigParams.SdrnServerInstance);
        }
        public static string GetRabbitMQHost(this IBusGateConfig config)
        {
            return config.GetValue<string>(ConfigParams.RabbitMQHost);
        }
        public static string GetRabbitMQPort(this IBusGateConfig config)
        {
            return config.GetValue<string>(ConfigParams.RabbitMQPort);
        }
    }
}
