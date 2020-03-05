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
using System.Threading;
using System.Threading.Tasks;
using Atdi.Modules.Sdrn.MessageBus;
using MD = Atdi.DataModels.Sdrns.Server.Entities;

namespace Atdi.AppUnits.Sdrn.BusController
{
    public class MessageProcessing : IDisposable
    {
        private readonly IMessagesSite _messagesSite;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;
        private Thread _processingThread;
        private EventWaitHandle _waitHandle;
        private CancellationToken _cancellationToken;
        private CancellationTokenSource _tokenSource;
        private int _waitTimeout = 1000 * 5;
		private readonly IQueryBuilder<MD.IAmqpMessageLog> _amqpMessageLogQueryBuilder;

		public MessageProcessing(
            IMessagesSite messagesSite,
            IDataLayer<EntityDataOrm> dataLayer,
            IEventEmitter eventEmitter,
            ILogger logger)
        {
            this._messagesSite = messagesSite;
            this._dataLayer = dataLayer;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
            this._waitHandle = new AutoResetEvent(false);
            this._tokenSource = new CancellationTokenSource();
            this._cancellationToken = this._tokenSource.Token;
            this._amqpMessageLogQueryBuilder = this._dataLayer.GetBuilder<MD.IAmqpMessageLog>();
		}

        public void OnCreatedMessage()
        {
            this._waitHandle.Set();
        }

        public void Run()
        {
            this._processingThread = new Thread(this.Process)
            {
                Name = $"ATDI.SdrnServer.DeviceBus.MessageProcessing"
            };

            this._processingThread.Start();

            _logger.Verbouse(Contexts.ThisComponent, Categories.ThreadRunning, Events.RanAmqpMessageProcessingThread);
        }

        private void Process()
        {
            try
            {
                while(!_cancellationToken.IsCancellationRequested)
                {
                    if (!this.HandleMessages())
                    {
                        _waitHandle.WaitOne(_waitTimeout);
                    }
                }
            }
            catch(ThreadAbortException)
            {
                Thread.ResetAbort();
                _logger.Critical(Contexts.ThisComponent, Categories.Processing, Events.AbortAmqpMessageProcessingThread);
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Processing, Events.AmqpMessageProcessingError, e, (object)this);
            }
            _waitHandle.Dispose();
            _waitHandle = null;
        }

        private bool HandleMessages()
        {
            try
            {
	            using (var dbScope = _dataLayer.CreateScope<SdrnServerDataContext>())
	            {


		            var messages = _messagesSite.GetMessagesForNotification();
		            if (messages == null || messages.Length == 0)
		            {
			            return false;
		            }

		            for (int i = 0; i < messages.Length; i++)
		            {
			            var m = messages[i];

			            this.HandleMessage(m.Item1, m.Item2, dbScope);

			            if (this._cancellationToken.IsCancellationRequested)
			            {
				            return true;
			            }
		            }
	            }

	            return true;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.MessagesHandling, e, (object)this);
                return false;
            }
        }

        private void HandleMessage(long id, string type, IDataLayerScope dbScope)
        {
            var statusCode = (byte)MessageProcessingStatus.SentEvent;
            var statusName = "SentEvent";
            var eventName = $"On{type}DeviceBusEvent";
            var statusNote = $"The event '{eventName}' was sent";
            try
            {
				this.AddToLog(id, statusCode, statusName, "Before sending event and changing status", type, dbScope);

                var busEvent = new DevicesBusEvent(eventName, "SdrnDeviceBusController")
                {
                    BusMessageId = id
                };
                _eventEmitter.Emit(busEvent);
            }
            catch (Exception e)
            {
                statusCode = (byte)MessageProcessingStatus.Failure;
                statusName = "Failure";
				statusNote = $"An error occurred while sending a notification '{eventName}': {e.Message}";
                _logger.Exception(Contexts.ThisComponent, Categories.Processing, statusNote, e, (object)this);
            }
            finally
            {
                _messagesSite.ChangeStatus(id, (byte)MessageProcessingStatus.Created, statusCode, statusNote);
            }
            
        }

        private void AddToLog(long messageId, byte statusCode, string statusName,  string statusNote, string messageType, IDataLayerScope dbScope)
        {
	        var logQueryInsert = this._amqpMessageLogQueryBuilder
		        .Insert()
		        .SetValue(c => c.MESSAGE.Id, messageId)
		        .SetValue(c => c.StatusCode, (byte)statusCode)
		        .SetValue(c => c.StatusName, statusName)
		        .SetValue(c => c.StatusNote, statusNote)
		        .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
		        .SetValue(c => c.ThreadId, System.Threading.Thread.CurrentThread.ManagedThreadId)
		        .SetValue(c => c.Source, $"SDRN.MessageProcessing:[Type='{messageType}'][HandleMessage]");
	        dbScope.Executor.Execute(logQueryInsert);
		}

        public void Dispose()
        {
            this._tokenSource.Cancel();
        }
    }
}
