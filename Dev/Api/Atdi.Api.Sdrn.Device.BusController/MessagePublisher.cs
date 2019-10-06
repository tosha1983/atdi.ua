using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using Atdi.Modules.Sdrn.DeviceBus;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal class MessagePublisher : IMessagePublisher
    {
        private readonly BusLogger _logger;
        private readonly DeviceBusConfig _config;
        private readonly AmqpPublisher _amqpPublisher;
        private readonly IBufferProcessing _buffer;
        private readonly BusMessagePacker _messagePacker;

        public string Tag { get; }

        internal MessagePublisher(string tag, DeviceBusConfig config, IBufferProcessing buffer, AmqpPublisher amqpPublisher, BusMessagePacker messagePacker, BusLogger logger)
        {
            this.Tag = tag;
            this._logger = logger;
            this._config = config;
            this._buffer = buffer;
            this._amqpPublisher = amqpPublisher;
            this._messagePacker = messagePacker;
        }

        public void Dispose()
        {
        }

        public IMessageToken Send<TObject>(string messageType, TObject messageObject, string correlationToken = null)
        {
            if (string.IsNullOrEmpty(messageType))
            {
                throw new ArgumentNullException(nameof(messageType));
            }
            if (string.IsNullOrEmpty(_config.SdrnServerInstance))
            {
                throw new ArgumentException("Undefined a server instance");
            }
            if (string.IsNullOrEmpty(_config.SdrnDeviceSensorName))
            {
                throw new ArgumentException("Undefined a sensor name");
            }
            if (string.IsNullOrEmpty(_config.SdrnDeviceSensorTechId))
            {
                throw new ArgumentException("Undefined a sensor tech ID");
            }

            try
            {
                var message = this._messagePacker.Pack<TObject>(messageType, messageObject, _config.SdrnServerInstance, _config.SdrnDeviceSensorName, _config.SdrnDeviceSensorTechId, correlationToken);
                var token = new MessageToken(message.Id, message.Type);

                if (_config.BufferConfig.Type == BufferType.Filesystem)
                {
                    this._buffer.Save(message);
                    this._logger.Verbouse("DeviceBus.MessageSending", $"The message is saved in the buffer: {message}, Buffer={_config.BufferConfig.Type}", this);
                }
                else if (_amqpPublisher.TryToSend(message))
                {
                    this._logger.Verbouse("DeviceBus.MessageSending", $"The message is sent immediately: {message}", this);
                    return token;
                }
                else
                {
                    throw new InvalidOperationException("The Device Bus is not available");
                }

                return token;
            }
            catch (Exception e)
            {
                var message = $"Failed to send message: {messageType}";

                this._logger.Exception("DeviceBus.MessageSending", message, e, this);
                throw new InvalidOperationException(message, e);
            }

        }

        
    }
}
