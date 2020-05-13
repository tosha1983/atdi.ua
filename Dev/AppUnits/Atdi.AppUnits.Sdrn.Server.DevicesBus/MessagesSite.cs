using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.Sdrns.Server.Events;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server.DevicesBus
{
    public class MessagesSite : IMessagesSite
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IEventEmitter _eventEmitter;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ILogger _logger;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IQueryBuilder<IAmqpMessage> _amqpMessageQueryBuilder;
        private readonly IQueryBuilder<IAmqpMessageLog> _amqpMessageLogQueryBuilder;
		private readonly IQueryBuilder<IAmqpEvent> _amqpEventQueryBuilder;

        public MessagesSite(IDataLayer<EntityDataOrm> dataLayer, IEventEmitter eventEmitter, ISdrnServerEnvironment environment, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._eventEmitter = eventEmitter;
            this._environment = environment;
            this._logger = logger;
            this._queryExecutor = this._dataLayer.Executor<SdrnServerDataContext>();
            this._amqpMessageQueryBuilder = this._dataLayer.GetBuilder<IAmqpMessage>();
            this._amqpMessageLogQueryBuilder = this._dataLayer.GetBuilder<IAmqpMessageLog>();
			this._amqpEventQueryBuilder = this._dataLayer.GetBuilder<IAmqpEvent>();
        }

        public IMessageProcessingScope<TDeliveryObject> StartProcessing<TDeliveryObject>(long messageId)
        {
            return new MessageProcessingScope<TDeliveryObject>(messageId, _dataLayer, _environment, _logger);
        }

        //public void ChangeStatus(long messageId, byte oldCode, byte newCode, string statusNote)
        //{
        //    try
        //    {
	       //     using (var dbScope = _dataLayer.CreateScope<SdrnServerDataContext>())
	       //     {
		      //      ChangeStatus(messageId, oldCode, newCode, statusNote);
	       //     }
        //    }
        //    catch (Exception e)
        //    {
        //        var error = $"An error occurred while changing status from {oldCode} to {newCode} for the message with id #{messageId}";
        //        _logger.Exception(Contexts.ThisComponent, Categories.Processing, error, e, (object)this);
        //        throw new InvalidOperationException(error, e);
        //    }
        //}

        //public ValueTuple<long, string>[] GetMessagesForNotification()
        //{
        //    try
        //    {
        //        var query = this._amqpEventQueryBuilder
        //        .From()
        //        .Select(
        //            c => c.Id,
        //            c => c.PropType
        //            )
        //        .OrderByAsc(c => c.Id);

        //        var result = new List<ValueTuple<long, string>>();

        //        this._queryExecutor.Fetch(query, reader =>
        //        {
        //            while(reader.Read())
        //            {
        //                var record = new ValueTuple<long, string>(reader.GetValue(c => c.Id), reader.GetValue(c => c.PropType));
        //                result.Add(record);
        //            }
        //            return true;
        //        });

        //        return result.ToArray();
        //    }
        //    catch (Exception e)
        //    {
        //        var error = $"An error occurred while retrieving database records of AMQP Messages for notificstion";
        //        _logger.Exception(Contexts.ThisComponent, Categories.Processing, error, e, (object)this);
        //        throw new InvalidOperationException(error, e);
        //    }
        //}

		private class MessageEventData
		{
			public long Id;
			public string Type;
		}

        public void RedirectMessages()
        {
	        try
	        {
		        using (var dbScope = _dataLayer.CreateScope<SdrnServerDataContext>())
		        {
			        while (true)
			        {
						var messages = GetNewMessages(dbScope);
						if (messages.Length == 0)
						{
							break;
						}
						for (int i = 0; i < messages.Length; i++)
						{
							this.ProcessMessage(dbScope, messages[i]);
						}
					}
		        }

			}
	        catch (Exception e)
	        {
				var error = $"An error occurred while processing AMQP Messages";
				_logger.Exception(Contexts.ThisComponent, Categories.Processing, error, e, (object)this);
	        }
        }

        private MessageEventData[] GetNewMessages(IDataLayerScope dbScope)
        {
	        var query = this._amqpEventQueryBuilder
		        .From()
		        .Select(
			        c => c.Id,
			        c => c.PropType
		        )
		        .OrderByAsc(c => c.Id);

	        var result = new List<MessageEventData>();

	        dbScope.Executor.Fetch(query, reader =>
	        {
		        while (reader.Read())
		        {
			        var record = new MessageEventData()
			        {
				        Id = reader.GetValue(c => c.Id),
				        Type = reader.GetValue(c => c.PropType)
			        };
			        result.Add(record);
		        }
		        return true;
	        });

	        return result.ToArray();
        }

        private void ProcessMessage(IDataLayerScope dbScope, MessageEventData message)
        {
	        if (message == null) throw new ArgumentNullException(nameof(message));

	        var eventName = $"On{message.Type}DeviceBusEvent";
	        
	        try
	        {
		        this.AddToLog(message.Id, MessageProcessingStatus.SentEvent, "Before sending event and changing status", $"SDRN.MessageProcessing:[Type='{message.Type}'][ProcessMessage]", dbScope);

				var busEvent = new DevicesBusEvent(eventName, "MessagesSite")
		        {
			        BusMessageId = message.Id
				};
		        _eventEmitter.Emit(busEvent);

		        
		        var deleteQuery = this._amqpEventQueryBuilder
			        .Delete()
			        .Where(c => c.Id, DataModels.DataConstraint.ConditionOperator.Equal, message.Id);
		        dbScope.Executor.Execute(deleteQuery);

		        var statusNote = $"The event '{eventName}' was sent";
				this.ChangeStatus(dbScope, message.Id, (byte)MessageProcessingStatus.Created, MessageProcessingStatus.SentEvent, statusNote);
			}
	        catch (Exception e)
	        {
		        var errorMessage = $"An error occurred while sending a notification '{eventName}': {e.Message}";
		        _logger.Exception(Contexts.ThisComponent, Categories.Processing, errorMessage, e, (object)this);
		        this.ChangeStatus(dbScope, message.Id, (byte)MessageProcessingStatus.Created, MessageProcessingStatus.Failure, errorMessage);

				throw new InvalidOperationException(errorMessage, e);
			}
        }

		private void ChangeStatus(IDataLayerScope dbScope, long messageId, MessageProcessingStatus oldCode, MessageProcessingStatus newCode, string statusNote)
		{
			try
			{
				var statusName = newCode.ToString();

				var updateQuery = this._amqpMessageQueryBuilder
					.Update()
					.SetValue(c => c.StatusCode, (byte)newCode)
					.SetValue(c => c.StatusName, statusName)
					.SetValue(c => c.StatusNote, statusNote)
					.Where(c => c.Id, DataModels.DataConstraint.ConditionOperator.Equal, messageId)
					// важно: следующее услови это блокиратор от ситуации когда 
					// евент дошел и начал обрабатывать на прикладном уровне сообщение
					.Where(c => c.StatusCode, DataModels.DataConstraint.ConditionOperator.Equal, (byte)oldCode);

				dbScope.Executor.Execute(updateQuery);

				
				this.AddToLog(messageId, newCode, $"Status was changed. Note: {statusNote}", $"SDRN.ChangeStatus:[OldCode=#{oldCode}][MessagesSite][ChangeStatus]", dbScope);
					
			}
			catch (Exception e)
			{
				var error = $"An error occurred while changing status from {oldCode} to {newCode} for the message with id #{messageId}";
				_logger.Exception(Contexts.ThisComponent, Categories.Processing, error, e, (object)this);
				throw new InvalidOperationException(error, e);
			}
		}

		private void AddToLog(long messageId, MessageProcessingStatus statusCode, string statusNote, string source, IDataLayerScope dbScope)
        {
	        var statusName = statusCode.ToString();
			var logQueryInsert = this._amqpMessageLogQueryBuilder
		        .Insert()
		        .SetValue(c => c.MESSAGE.Id, messageId)
		        .SetValue(c => c.StatusCode, (byte)statusCode)
		        .SetValue(c => c.StatusName, statusName)
		        .SetValue(c => c.StatusNote, statusNote)
		        .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
		        .SetValue(c => c.ThreadId, System.Threading.Thread.CurrentThread.ManagedThreadId)
		        .SetValue(c => c.Source, source);
	        dbScope.Executor.Execute(logQueryInsert);
        }
	}
}
