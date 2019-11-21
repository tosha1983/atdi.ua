using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Modules.AmqpBroker;

namespace Atdi.Api.Sdrn.Device.BusController
{
    [Serializable]
    internal sealed class BufferedMessage
    {
        [Serializable]
        public sealed class DeliveryContext
        {
            public DeliveryContext()
            {
            }

            public DeliveryContext(IDeliveryContext context)
            {
                this.ConsumerTag = context.ConsumerTag;
                this.DeliveryTag = context.DeliveryTag;
                this.Redelivered = context.Redelivered;
                this.Exchange = context.Exchange;
                this.RoutingKey = context.RoutingKey;
                this.Channel = context?.Channel.Number.ToString();
                this.Queue = context.Queue;
            }

            public string ConsumerTag;

            public string DeliveryTag;

            public bool Redelivered;

            public string Exchange;

            public string RoutingKey;

            public string Channel;

            public string Queue;
        }

        public BufferedMessage()
        {

        }

        public BufferedMessage(IDeliveryMessage source, IDeliveryContext context)
        {
            this.Id = source.Id;
            this.Type = source.Type;
            this.AppId = source.AppId;
            this.ContentType = source.ContentType;
            this.ContentEncoding = source.ContentEncoding;
            this.CorrelationId = source.CorrelationId;
            this.Protocol = source.GetHeaderValue(Modules.Sdrn.DeviceBus.Protocol.Header.Protocol);
            this.ApiVersion = source.GetHeaderValue(Modules.Sdrn.DeviceBus.Protocol.Header.ApiVersion);
            this.BodyAqName = source.GetHeaderValue(Modules.Sdrn.DeviceBus.Protocol.Header.BodyAqName);
            this.Created = source.GetHeaderValue(Modules.Sdrn.DeviceBus.Protocol.Header.Created);
            this.Server = source.GetHeaderValue(Modules.Sdrn.DeviceBus.Protocol.Header.SdrnServer);
            this.Sensor = source.GetHeaderValue(Modules.Sdrn.DeviceBus.Protocol.Header.SensorName);
            this.TechId = source.GetHeaderValue(Modules.Sdrn.DeviceBus.Protocol.Header.SensorTechId);
            this.Body = source.Body;
            this.Context = new DeliveryContext(context);
        }

        public string Id;

        public string Type;

        public string ApiVersion;

        public string AppId;

        public string ContentType;

        public string ContentEncoding;

        public string CorrelationId;

        public string Sensor;

        public string TechId;

        public string Server;

        public string Protocol;

        public string Created;

        public string BodyAqName;

        public byte[] Body;

        public DeliveryContext Context;
    }
}
