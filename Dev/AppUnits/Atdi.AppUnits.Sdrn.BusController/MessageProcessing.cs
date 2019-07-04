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
        }

        public void OnCreatedMessage()
        {
            this._waitHandle.Set();
        }

        public void Run()
        {
            this._processingThread = new Thread(this.Process)
            {
                Name = $"SDRN.BusController.AMQPMessages.Processing"
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
                var messages = _messagesSite.GetMessagesForNotification();
                if (messages == null || messages.Length == 0)
                {
                    return false;
                }

                for (int i = 0; i < messages.Length; i++)
                {
                    var m = messages[i];

                    this.HandleMessage(m.Item1, m.Item2);

                    if (this._cancellationToken.IsCancellationRequested)
                    {
                        return true;
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

        private void HandleMessage(long id, string type)
        {
            var statusCode = (byte)0;
            var statusNote = string.Empty;
            try
            {
                var busEvent = new DevicesBusEvent($"On{type}DeviceBusEvent", "SdrnDeviceBusController")
                {
                    BusMessageId = id
                };

                _eventEmitter.Emit(busEvent);

                statusCode = 1;
            }
            catch (Exception e)
            {
                statusCode = 4;
                statusNote = $"An error occurred while sending a notification 'On{type}DeviceBusEvent': {e.Message}";
                _logger.Exception(Contexts.ThisComponent, Categories.Processing, statusNote, e, (object)this);
            }
            finally
            {
                _messagesSite.ChangeStatus(id, (byte)0, statusCode, statusNote);
            }
            
        }

        public void Dispose()
        {
            this._tokenSource.Cancel();
        }
    }
}
