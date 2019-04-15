using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.Api.DataBus
{
    internal sealed class Publisher : IPublisher
    {
        private readonly IBusConfig _config;
        private readonly AmqpPublisher _amqpPublisher;
        private readonly IBufferProcessing _buffer;
        private readonly MessagePacker _messagePacker;
        private readonly BusLogger _logger;

        public Publisher(IBusConfig config, IBufferProcessing buffer, AmqpPublisher amqpPublisher, MessagePacker messagePacker, BusLogger logger)
        {
            this._config = config;
            this._buffer = buffer;
            this._amqpPublisher = amqpPublisher;
            this._messagePacker = messagePacker;
            this._logger = logger;
        }

        public IOutgoingEnvelope<TMessageType, TDeliveryObject> CreateEnvelope<TMessageType, TDeliveryObject>() where TMessageType : IMessageType, new()
        {
            return new OutgoingEnvelope<TMessageType, TDeliveryObject>(this._config);
        }

        public void Dispose()
        {
            
        }

        public void Send<TMessageType, TDeliveryObject>(IOutgoingEnvelope<TMessageType, TDeliveryObject> envelope) where TMessageType : IMessageType, new()
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            if (string.IsNullOrEmpty(envelope.To))
            {
                throw new ArgumentException("Undefined address To", nameof(envelope));
            }

            try
            {
                var message = this._messagePacker.Pack(envelope);

                if (_amqpPublisher.TryToSend(message))
                {
                    this._logger.Verbouse(BusContexts.EnvelopeSending, $"The envelope was sent immediately: {envelope}", this);
                    return;
                }

                if ((envelope.Options & SendingOptions.UseBuffer) == SendingOptions.UseBuffer)
                {
                    this._buffer.Save(message);
                    this._logger.Verbouse(BusContexts.EnvelopeSending, $"The Data Bus is not available. The envelope is stored in the buffer: {envelope}", this);
                }
                else
                {
                    throw new InvalidOperationException("The Data Bus is not available");
                }
            }
            catch (Exception e)
            {
                var message = $"Failed to send envelope: {envelope}";

                this._logger.Exception(BusContexts.EnvelopeSending, message,  e, this);
                throw new InvalidOperationException(message, e);
            }
        }

        
    }
}
