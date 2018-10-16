using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns;
using Atdi.Modules.Sdrn.AmqpBroker;
using Atdi.Modules.Sdrn.MessageBus;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.BusController
{
    public class SdrnMessagePublisher : ISdrnMessagePublisher
    {
        private readonly SdrnBusControllerConfig _busControllerConfig;
        private readonly ISdrnServerEnvironment _environment;
        private readonly MessageConverter _messageConverter;
        private readonly ILogger _logger;
        private readonly string _serverExchangeName;
        private BusConnection _busConnection;

        public SdrnMessagePublisher(SdrnBusControllerConfig busControllerConfig, ISdrnServerEnvironment environment, BusConnectionFactory busConnectionFactory, MessageConverter messageConverter, ILogger logger)
        {
            this._busControllerConfig = busControllerConfig;
            this._environment = environment;
            this._messageConverter = messageConverter;
            this._logger = logger;

            this.Tag = $"[SDRN.Server].[{environment.ServerInstance}].[Publisher].[#{System.Threading.Thread.CurrentThread.ManagedThreadId}]";

            var busConfig = new BusConnectionConfig
            {
                ApplicationName = environment.GetAppName(),
                ConnectionName = this.Tag,
                AutoRecovery = true,
                HostName = busControllerConfig.BusHost,
                VirtualHost = busControllerConfig.BusVirtualHost,
                Port = busControllerConfig.BusPort,
                UserName = busControllerConfig.BusUser,
                Password = busControllerConfig.BusPassword
            };

            this._busConnection = busConnectionFactory.Create(busConfig);

            this._serverExchangeName = $"{this._busControllerConfig.ServerExchange}.[v{this._busControllerConfig.ApiVersion}]";
            this._busConnection.DeclareDurableDirectExchange(this._serverExchangeName);

        }

        public string Tag { get; }

        public ISdrnOutgoingEnvelope<TMessageType, TDeliveryObject> CreateOutgoingEnvelope<TMessageType, TDeliveryObject>() 
            where TMessageType : SdrnBusMessageType<TDeliveryObject>, new()
        {
            return new SdrnOutgoingEnvelope<TMessageType, TDeliveryObject>();
        }

        public void Dispose()
        {
            if (_busConnection != null)
            {
                _busConnection.Dispose();
                _busConnection = null;
            }
        }

        private QueueDescriptor DeclareDeviceQueue(string sensorName, string sensorTechId)
        {
            var queueDescriptor = new QueueDescriptor
            {
                Name = $"{this._busControllerConfig.DeviceQueueNamePart}.[{sensorName}].[{sensorTechId}].[v{this._busControllerConfig.ApiVersion}]",
                RoutingKey = $"[{sensorName}].[{sensorTechId}]",
                Exchange = this._serverExchangeName
            };

            this._busConnection.DeclareDurableQueue(queueDescriptor.Name, queueDescriptor.Exchange, queueDescriptor.RoutingKey);
            return queueDescriptor;

        }

        public string Send<TMessageType, TDeliveryObject>(ISdrnOutgoingEnvelope<TMessageType, TDeliveryObject> envelope) 
            where TMessageType : SdrnBusMessageType<TDeliveryObject>, new()
        {
            var message = this._messageConverter.Pack<TDeliveryObject>(envelope.MessageType.Name, envelope.DeliveryObject);
            message.CorrelationId = envelope.CorrelationToken;

            message.Headers = new Dictionary<string, object>
            {
                ["SdrnServer"] = _environment.ServerInstance,
                ["SensorName"] = envelope.SensorName,
                ["SensorTechId"] = envelope.SensorTechId,
                ["Created"] = DateTime.Now.ToString("o")
            };

            var queueDescriptor = DeclareDeviceQueue(envelope.SensorName, envelope.SensorTechId);

            var result = _busConnection.Publish(queueDescriptor.Exchange, queueDescriptor.RoutingKey, message);
            return result;
        }
    }
}
