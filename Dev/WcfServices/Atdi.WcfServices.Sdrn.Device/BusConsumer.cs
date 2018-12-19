using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;
using RabbitMQ.Client;
using MMB = Atdi.Modules.Sdrn.MessageBus;
using Atdi.DataModels.Sdrns.Device;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Atdi.WcfServices.Sdrn.Device
{
    class BusConsumer : DefaultBasicConsumer, IDisposable
    {
        private readonly SdrnServerDescriptor _serverDescriptor;
        private readonly ILogger _logger;
        private readonly string _tag;
        private IModel _channel;
        private long _fileCounter = 0;

        public BusConsumer(string tag, IModel channel, SdrnServerDescriptor serverDescriptor, ILogger logger)
            : base(channel)
        {
            this._serverDescriptor = serverDescriptor;
            this._logger = logger;
            this._tag = tag;
            this._channel = channel;
        }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"BusConsumer.HandleBasicDeliver", $"The message was received: consumerTag = '{consumerTag}', deliveryTag = '{deliveryTag}', exchange = '{exchange}', routingKey = '{routingKey}', Type = '{properties.Type}'");
            try
            {
                if (!properties.IsTypePresent() || !properties.IsMessageIdPresent() || !properties.IsAppIdPresent())
                {
                    throw new InvalidOperationException("Incorrect message properties: Id or Type or AppId is not present");
                }

                var messageData = new MessageData
                {
                    Created = DateTime.Now,
                    Descriptor = new SensorDescriptor
                    {
                        SdrnServer = GetHeaderValue(properties, "SdrnServer"),
                        SensorName = GetHeaderValue(properties, "SensorName"),
                        EquipmentTechId = GetHeaderValue(properties, "SensorTechId")
                    },
                    Message = new MMB.Message
                    {
                        Id = properties.MessageId,
                        Type = properties.Type,
                        ContentType = properties.ContentType,
                        ContentEncoding = properties.ContentEncoding,
                        CorrelationId = properties.CorrelationId,
                        Headers = properties.Headers,
                        Body = body
                    }
                };

                if (string.IsNullOrEmpty(messageData.Descriptor.SdrnServer))
                {
                    throw new InvalidOperationException("Incorrect message properties: SdrnServer is not present");
                }
                if (string.IsNullOrEmpty(messageData.Descriptor.SensorName))
                {
                    throw new InvalidOperationException("Incorrect message properties: SensorName is not present");
                }
                if (string.IsNullOrEmpty(messageData.Descriptor.EquipmentTechId))
                {
                    throw new InvalidOperationException("Incorrect message properties: EquipmentTechId is not present");
                }

                var fileName = MakeFileName();
                var directory = Path.Combine(this._serverDescriptor.MessagesInboxFolder, PrepareTypeForFolderName(properties.Type));

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var fullPath = Path.Combine(directory, fileName);

                IFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    formatter.Serialize(stream, messageData);
                    File.WriteAllBytes(fullPath, stream.ToArray());
                }

                _channel.BasicAck(deliveryTag, false);
                
                this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"BusConsumer.HandleBasicDeliver", $"The received message was saved successfully: Type = '{properties.Type}', Id = '{properties.MessageId}', SDRN Server = '{messageData.Descriptor.SdrnServer}', Sensor = '{messageData.Descriptor.SensorName}', File = '{fileName}'");
            }
            catch(Exception e)
            {
                this._logger.Exception("SdrnDeviceServices", (EventCategory)"BusConsumer.HandleBasicDeliver", e);
                _channel.BasicNack(deliveryTag, false, true);
            }
        }

        public static string PrepareTypeForFolderName(string typeName)
        {
            return Path.Combine("ByTypes",  typeName);
        }
        private string MakeFileName()
        {
            var d = DateTime.Now;

            var timeFormat = "yyyyMMdd_HHmmss";
            var timeString = d.ToString(timeFormat);


            var timeFormat3 = "FFFFFFF";
            var timeString3 = d.ToString(timeFormat3).PadRight(timeFormat3.Length, '0');

            System.Threading.Interlocked.Increment(ref this._fileCounter);
            return timeString + "_" + timeString3 + "_"  + (this._fileCounter).ToString().PadLeft(10, '0') + ".mdata";
        }

        private string GetHeaderValue(IBasicProperties properties, string key)
        {
            var headers = properties.Headers;
            if (headers == null)
            {
                return null;
            }
            if (!headers.ContainsKey(key))
            {
                return null;
            }
            return Convert.ToString(Encoding.UTF8.GetString(((byte[])headers[key])));
        }
        public void Dispose()
        {
            if (this._channel != null)
            {
                if (this._channel.IsOpen)
                {
                    this._channel.BasicCancel(this._tag);
                    this._channel.Close();
                }
                this._channel.Dispose();
                this._channel = null;
            }
        }
    }
}
