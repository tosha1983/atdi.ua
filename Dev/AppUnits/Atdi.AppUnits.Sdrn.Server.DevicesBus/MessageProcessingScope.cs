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
using Atdi.Modules.Sdrn.DeviceBus;

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

            //public string HeaderApiVersion;

            public string HeaderProtocol;

            public string HeaderBodyAqName;

        }
        private readonly long _messageId;
        private string _messageType;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ILogger _logger;
        private bool _isDisposed = false;
        private readonly string _sharedSecretKey;
        private IDataLayerScope _dbScope;

		public MessageProcessingScope(long messageId, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ILogger logger)
        {
            this._messageId = messageId;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._logger = logger;
            this._sharedSecretKey = _environment.GetSharedSecretKey("DeviceBus");
            this.LoadMessageFromStorage();
        }

        private void LoadMessageFromStorage()
        {
	        _dbScope = _dataLayer.CreateScope<SdrnServerDataContext>();
	        
	        var queryExecutor = _dbScope.Executor;

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
			        c => c.PropContentEncoding,
			        c => c.HeaderBodyAQName,
			        c => c.HeaderProtocol,
					c => c.PropType
		        )
		        .Where(c => c.Id, DataModels.DataConstraint.ConditionOperator.Equal, _messageId);

			var amqpMessage = new AmqpMessage();
	
	        queryExecutor.Fetch(query, reader =>
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
		        amqpMessage.HeaderBodyAqName = reader.GetValue(c => c.HeaderBodyAQName);
		        amqpMessage.HeaderProtocol = reader.GetValue(c => c.HeaderProtocol);
		        _messageType = reader.GetValue(c => c.PropType);

				return true;
	        });

	        if (amqpMessage.StatusCode == (byte) MessageProcessingStatus.Processing)
	        {
		        throw new InvalidOperationException(
			        $"The message with ID #{_messageId} has invalid status code #{amqpMessage.StatusCode}");
	        }


	        this.SensorName = amqpMessage.HeaderSensorName;
	        this.SensorTechId = amqpMessage.HeaderSensorTechId;
	        this.Status = MessageProcessingStatus.Processing;

	        var content = amqpMessage.BodyContent;
	        var encoding = Protocol.ContentEncoding.Decode(amqpMessage.BodyContentEncoding);
	        if (encoding.UseCompression)
	        {
		        content = Compressor.Decompress(content);
	        }

	        // пока тело сообщение в первичном виде (тип Json временный, будет исключен через версию.)
	        if (Protocol.ContentType.QualifiedOriginal.Equals(amqpMessage.BodyContentType,
		            StringComparison.OrdinalIgnoreCase)
	            || Protocol.ContentType.Json.Equals(amqpMessage.BodyContentType,
		            StringComparison.OrdinalIgnoreCase))
	        {
		        this.Delivery = (TDeliveryObject) BusMessagePacker.Unpack(amqpMessage.PropContentType,
			        amqpMessage.PropContentEncoding,
			        amqpMessage.HeaderProtocol, content, amqpMessage.HeaderBodyAqName, _sharedSecretKey);
		        //this.Deserialize<TDeliveryObject>(content, amqpMessage.PropContentType, amqpMessage.PropContentEncoding);
	        }
	        else
	        {
		        throw new InvalidOperationException(
			        $"Unsupported  body content type '{amqpMessage.BodyContentType}'");
	        }

	        var updateQuery = this._dataLayer.GetBuilder<IAmqpMessage>()
		        .Update()
		        .SetValue(c => c.StatusCode, (byte) MessageProcessingStatus.Processing)
		        .SetValue(c => c.ProcessedStartDate, DateTimeOffset.Now)
		        .Where(c => c.Id, DataModels.DataConstraint.ConditionOperator.Equal, _messageId);

	        queryExecutor.Execute(updateQuery);

	        var logQueryInsert = this._dataLayer.GetBuilder<IAmqpMessageLog>()
				.Insert()
		        .SetValue(c => c.MESSAGE.Id, _messageId)
		        .SetValue(c => c.StatusCode, (byte)MessageProcessingStatus.Processing)
		        .SetValue(c => c.StatusName, "Processing")
		        .SetValue(c => c.StatusNote, $"Status was changed.")
		        .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
		        .SetValue(c => c.ThreadId, System.Threading.Thread.CurrentThread.ManagedThreadId)
		        .SetValue(c => c.Source,
			        $"SDRN.ChangeStatus:[{_messageType}][MessageProcessingScope][LoadMessageFromStorage]");
	        queryExecutor.Execute(logQueryInsert);
	        
        }

        public string SensorName { get; set; }

        public string SensorTechId { get; set; }

        public TDeliveryObject Delivery { get; set; }

        public MessageProcessingStatus Status { get; set; }

        public string ResultNote { get; set; }

     

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

	            var queryExecutor = _dbScope.Executor;

	            var updateQuery = this._dataLayer.GetBuilder<IAmqpMessage>()
		            .Update()
		            .SetValue(c => c.StatusCode, (byte) this.Status)
		            .SetValue(c => c.StatusName, this.Status.ToString())
		            .SetValue(c => c.StatusNote, this.ResultNote)
		            .SetValue(c => c.ProcessedFinishDate, DateTimeOffset.Now)
		            .Where(c => c.Id, DataModels.DataConstraint.ConditionOperator.Equal, _messageId);

	            queryExecutor.Execute(updateQuery);

	            var logQueryInsert = this._dataLayer.GetBuilder<IAmqpMessageLog>()
		            .Insert()
		            .SetValue(c => c.MESSAGE.Id, _messageId)
		            .SetValue(c => c.StatusCode, (byte) this.Status)
		            .SetValue(c => c.StatusName, this.Status.ToString())
		            .SetValue(c => c.StatusNote, $"Status was changed. Note: {this.ResultNote}")
		            .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
		            .SetValue(c => c.ThreadId, System.Threading.Thread.CurrentThread.ManagedThreadId)
		            .SetValue(c => c.Source,
			            $"SDRN.ChangeStatus:[{_messageType}][MessageProcessingScope][Dispose]");
	            queryExecutor.Execute(logQueryInsert);
            }
            catch (Exception e)
            {
	            _logger.Exception(Contexts.ThisComponent, Categories.Disposing, e, (object) this);
	            throw;
            }
            finally
            {
	            _dbScope?.Dispose();
	            _dbScope = null;

            }
        }
    }

    //public class MessageBody
    //{
    //    public string Type { get; set; }

    //    public string JsonBody { get; set; }
    //}
}
