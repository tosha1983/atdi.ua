using System;
using System.Threading;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Modules.AmqpBroker;
using Atdi.Modules.Sdrn.DeviceBus;
using Atdi.Modules.Sdrn.MessageBus;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal sealed class BusGate : IBusGate
    {
        private readonly BusLogger _logger;
        private readonly DeviceBusConfig _config;
        private readonly IBufferProcessing _bufferProcessing;
        private readonly ConnectionFactory _amqpConnectionFactory;
        private readonly BusMessagePacker _messagePacker;
        private readonly AmqpPublisher _amqpPublisher;

        internal BusGate(string tag, DeviceBusConfig config, BusLogger logger)
        {
            this.Tag = tag;
            this._logger = logger;
            this._config = config;

            this._messagePacker = new BusMessagePacker(new PackerOptions
            {
                ContentType = config.DeviceBusContentType,
                ApiVersion = config.SdrnApiVersion,
                Protocol = config.DeviceBusProtocol,
                Application = config.DeviceBusClient,
                SharedSecretKey = config.DeviceBusSharedSecretKey,
                UseEncryption = config.SdrnMessageConvertorUseEncryption,
                UseCompression = config.SdrnMessageConvertorUseCompression
            });

            this._amqpConnectionFactory = new ConnectionFactory(this._logger);
            this._amqpPublisher = new AmqpPublisher(config, this._amqpConnectionFactory, this._logger);

            if (_config.OutboxBufferConfig.Type == BufferType.Filesystem)
            {
                _bufferProcessing = new FileSystemBufferProcessing(this._config, this._amqpPublisher, this._messagePacker, this._logger);
            }
            else if (_config.OutboxBufferConfig.Type != BufferType.None)
            {
                throw new InvalidOperationException($"Unsupported the buffer type with name '{_config.OutboxBufferConfig.Type}'");
            }

            _bufferProcessing?.Start();
        }

        public string Tag { get; }

        public IBusGateConfig Config => this._config.GateConfig;

        public IMessageDispatcher CreateDispatcher(string dispatcherTag, IBusEventObserver eventObserver = null)
        {
            try
            {
                var logger = _logger;
                if (eventObserver != null)
                {
                    logger = new BusLogger(eventObserver);
                }

                var dispatcher = new MessageDispatcher(dispatcherTag, this._config, this._amqpConnectionFactory, this._messagePacker, logger);

                logger.Info(0, "DeviceBus.DispatcherCreation", $"The message dispatcher is created successfully: Tag='{dispatcherTag}', {this}", this);

                return dispatcher;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"The message dispatcher is not created: Tag='{dispatcherTag}', {this}", e);
            }
        }

        public IMessagePublisher CreatePublisher(string publisherTag, IBusEventObserver eventObserver = null)
        {
            try
            {
                var logger = _logger;
                if (eventObserver != null)
                {
                    logger = new BusLogger(eventObserver);
                }

                var publisher = new MessagePublisher(publisherTag, this._config, this._bufferProcessing, _amqpPublisher, _messagePacker, logger);

                logger.Info(0, "DeviceBus.PublisherCreation", $"The message publisher is created successfully: Tag='{publisherTag}', {this}", this);

                return publisher;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"The message publisher is not created: Tag='{publisherTag}', {this}", e);
            }
        }

        public void Dispose()
        {
            _bufferProcessing?.Stop();
            _amqpPublisher.Dispose();
            this._logger.Verbouse("DeviceBus.GateDisposing", $"The gate is disposed successfully: {this}", this);
        }

        public override string ToString()
        {
            return $"Gate='{Tag}'";
        }
    }
}
