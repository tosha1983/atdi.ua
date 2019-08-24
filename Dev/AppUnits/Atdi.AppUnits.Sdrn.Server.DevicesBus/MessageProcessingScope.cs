using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Platform.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server.DevicesBus
{
    class MessageProcessingScope<TDeliveryObject> : IMessageProcessingScope<TDeliveryObject>
    {
        private struct AmqpMessage
        {
            //public int Id;
            public byte StatusCode;
            //public string StatusNote;
            //public DateTimeOffset CreatedDate;
            //public int ThreadId;
            //public DateTimeOffset ProcessedStartDate;
            //public DateTimeOffset ProcessedFinishDate;
            //public string PropExchange;
            //public string PropRoutingKey;
            //public string PropAppId;
            //public string PropType;
            //public long? PropTimestamp;
            //public string PropMessageId;
            //public string PropCorrelationId;
            //public byte? PropDeliveryMode;
            //public string PropDeliveryTag;
            //public string PropConsumerTag;
            public string PropContentEncoding;
            public string PropContentType;
            //public string HeaderCreated;
            public string HeaderSdrnServer;
            public string HeaderSensorName;
            public string HeaderSensorTechId;
            public string BodyContentType;
            public string BodyContentEncoding;
            public byte[] BodyContent;
            
        }
        private readonly long _messageId;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;
        private bool _isDisposed = false;

        public MessageProcessingScope(long messageId, IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._messageId = messageId;
            this._dataLayer = dataLayer;
            this._logger = logger;
            this.LoadMessageFromStorage();
        }

        private void LoadMessageFromStorage()
        {
            var query = this._dataLayer.GetBuilder<IAmqpMessage>()
                .From()
                .Select(
                    c => c.BodyContent,
                    c => c.BodyContentEncoding,
                    c => c.BodyContentType,
                    c => c.StatusCode,
                    c => c.HeaderSdrnServer,
                    c => c.HeaderSensorName,
                    c => c.HeaderSensorTechId,
                    c => c.PropContentType,
                    c => c.PropContentEncoding
                    )
                .Where(c => c.Id, DataModels.DataConstraint.ConditionOperator.Equal, _messageId);

            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();

            AmqpMessage amqpMessage = new AmqpMessage();

            queryExecuter.Fetch(query, reader =>
            {
                if (!reader.Read())
                {
                    throw new InvalidOperationException($"Not found message with ID #{_messageId}");
                }

                amqpMessage.BodyContent = reader.GetValue(c => c.BodyContent);
                amqpMessage.BodyContentEncoding = reader.GetValue(c => c.BodyContentEncoding);
                amqpMessage.BodyContentType = reader.GetValue(c => c.BodyContentType);
                amqpMessage.StatusCode = reader.GetValue(c => c.StatusCode);
                amqpMessage.HeaderSdrnServer = reader.GetValue(c => c.HeaderSdrnServer);
                amqpMessage.HeaderSensorName = reader.GetValue(c => c.HeaderSensorName);
                amqpMessage.HeaderSensorTechId = reader.GetValue(c => c.HeaderSensorTechId);
                amqpMessage.PropContentType = reader.GetValue(c => c.PropContentType);
                amqpMessage.PropContentEncoding = reader.GetValue(c => c.PropContentEncoding);

                return true;
            });

            if (amqpMessage.StatusCode == 2)
            {
                throw new InvalidOperationException($"The message with ID #{_messageId} has invalid status code #{amqpMessage.StatusCode}");
            }


            this.SensorName = amqpMessage.HeaderSensorName;
            this.SensorTechId = amqpMessage.HeaderSensorTechId;
            this.Status = MessageProcessingStatus.Processing;

            var content = amqpMessage.BodyContent;

            if ("compressed".Equals(amqpMessage.BodyContentEncoding, StringComparison.OrdinalIgnoreCase) )
            {
                content = Compressor.Decompress(content);
            }
            if ("json".Equals(amqpMessage.BodyContentType, StringComparison.OrdinalIgnoreCase))
            {
                this.Delivery = this.Deserialize<TDeliveryObject>(content, amqpMessage.PropContentType, amqpMessage.PropContentEncoding);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported  body content type '{amqpMessage.BodyContentType}'");
            }

            var updateQuery = this._dataLayer.GetBuilder<IAmqpMessage>()
                .Update()
                .SetValue(c => c.StatusCode, (byte)2)
                .SetValue(c => c.ProcessedStartDate, DateTimeOffset.Now)
                .Where(c => c.Id, DataModels.DataConstraint.ConditionOperator.Equal, _messageId);

            queryExecuter.Execute(updateQuery);
        }

        public string SensorName { get; set; }

        public string SensorTechId { get; set; }

        public TDeliveryObject Delivery { get; set; }

        public MessageProcessingStatus Status { get; set; }

        public string ResultNote { get; set; }

        private T Deserialize<T>(byte[] content, string contentType, string contentEncoding)
        {

            var json = Encoding.UTF8.GetString(content);
            var type = typeof(T);
            
            if ("application/sdrn".Equals(contentType, StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(contentEncoding))
                {
                    if (contentEncoding.Contains("encrypted"))
                    {
                        json = Encryptor.Decrypt(json);
                    }
                    if (contentEncoding.Contains("compressed"))
                    {
                        // пока не подключен компрессор 
                        // json = json;
                    }
                }

                var messageBody = JsonConvert.DeserializeObject<MessageBody>(json);
                var msgObjectType = Type.GetType(messageBody.Type);
                if (type != msgObjectType)
                {
                    throw new InvalidOperationException($"Unexpected type '{msgObjectType.AssemblyQualifiedName}'. Expected type '{type.AssemblyQualifiedName}'");
                }
                json = messageBody.JsonBody;
            }
            else
            {
                throw new InvalidOperationException($"Unsupported content type '{contentType}'");
            }

            var result = JsonConvert.DeserializeObject(json, type);

            return (T)result;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;

            try
            {
                if (this.Status == MessageProcessingStatus.Processing)
                {
                    this.Status = MessageProcessingStatus.Failure;
                    this.ResultNote = "The message processing status not acknowledged";
                }

                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();

                var updateQuery = this._dataLayer.GetBuilder<IAmqpMessage>()
                .Update()
                .SetValue(c => c.StatusCode, (byte)this.Status)
                .SetValue(c => c.StatusNote, this.ResultNote)
                .SetValue(c => c.ProcessedFinishDate, DateTimeOffset.Now)
                .Where(c => c.Id, DataModels.DataConstraint.ConditionOperator.Equal, _messageId);

                queryExecuter.Execute(updateQuery);
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Disposing, e, (object)this);
                throw;
            }
        }
    }

    public class MessageBody
    {
        public string Type { get; set; }

        public string JsonBody { get; set; }
    }
}
