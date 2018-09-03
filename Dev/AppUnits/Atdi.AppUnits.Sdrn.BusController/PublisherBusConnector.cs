﻿using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Atdi.DataModels.Sdrns.Device;
using Newtonsoft.Json;

namespace Atdi.AppUnits.Sdrn.BusController
{
    public class PublisherBusConnector : LoggedObject, IDisposable
    {
        private readonly SdrnServerDescriptor _serverDescriptor;
        private readonly PublisherRabbitMQConnection _connection;
        private readonly string _serverExchangeName;

        public PublisherBusConnector(PublisherRabbitMQConnection connection, SdrnServerDescriptor serverDescriptor, ILogger logger) : base(logger)
        {
            this._connection = connection;
            this._serverDescriptor = serverDescriptor;

            this._serverExchangeName = $"{this._serverDescriptor.ServerExchange}.[v{this._serverDescriptor.ApiVersion}]";
            this._connection.DeclareDurableDirectExchange(this._serverExchangeName);
        }

        private QueueDescriptor DeclareDeviceQueue(SensorDescriptor descriptor)
        {
            var queueDescriptor = new QueueDescriptor
            {
                Name = $"{this._serverDescriptor.DeviceQueueNamePart}.[{descriptor.SensorName}].[{descriptor.EquipmentTechId}].[v{this._serverDescriptor.ApiVersion}]",
                RoutingKey = $"[{descriptor.SensorName}].[{descriptor.EquipmentTechId}]",
                Exchange = this._serverExchangeName
            };
            
            this._connection.DeclareDurableQueue(queueDescriptor);
            return queueDescriptor;

        }

        public string Publish<TObject>(SensorDescriptor descriptor, string type, TObject source, string correlationId = null)
        {
            var queue = this.DeclareDeviceQueue(descriptor);

            var body = SerializeObjectToByteArray(source);
            var headers = new Dictionary<string, object>
            {
                ["SdrnServer"] = descriptor.SdrnServer,
                ["SensorName"] = descriptor.SensorName,
                ["SensorTechId"] = descriptor.EquipmentTechId,
                ["Created"] = DateTime.Now.ToString("o")
            };

            return this._connection.Publish(queue.Exchange, queue.RoutingKey, headers, type, body, correlationId);
        }

        private byte[] SerializeObjectToByteArray<TObject>(TObject source)
        {
            if (source == null)
            {
                return new byte[] { };
            }

            byte[] result = null;

            var json = JsonConvert.SerializeObject(source);
            result = Encoding.UTF8.GetBytes(json);

            return result;
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
