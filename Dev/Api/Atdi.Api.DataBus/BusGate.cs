using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Atdi.Modules.AmqpBroker;

namespace Atdi.Api.DataBus
{
    internal sealed class BusGate : IBusGate
    {
        private readonly IBusConfig _config;
        private readonly BusLogger _logger;
        private readonly IBufferProcessing _bufferProcessing;
        private readonly ConnectionFactory _amqpConnectionFactory;
        private readonly MessagePacker _messagePacker;
        private readonly AmqpPublisher _amqpPublisher;

        public BusGate(IBusConfig config, IBusEventObserver eventObserver)
        {
            this._config = config ?? throw new ArgumentNullException(nameof(config));
            this._logger = new BusLogger(eventObserver);

            this._messagePacker = new MessagePacker(this._config);

            ValidateConfig();

            this._amqpConnectionFactory = new ConnectionFactory(this._logger);
            this._amqpPublisher = new AmqpPublisher(config, this._amqpConnectionFactory, this._logger);

            if (_config.Buffer.Type == BufferType.Filesystem)
            {
                _bufferProcessing = new FileSystemBufferProcessing(this._config, this._amqpPublisher, this._messagePacker, this._logger);
            }
            else if (_config.Buffer.Type != BufferType.None)
            {
                throw new InvalidOperationException($"Unsupported the buffer type with name '{_config.Buffer.Type}'");
            }

            if (_bufferProcessing != null)
            {
                _bufferProcessing.Start();
            }

            _logger.Verbouse(BusContexts.Initialization, $"The bus gate object was created: Bus = '{_config.Name}', Address = '{_config.Address}'", this);
        }

        private void ValidateConfig()
        {
            var result = new StringBuilder();

            if (string.IsNullOrEmpty(this.Config.Address))
            {
                result.AppendLine("Undefined Address");
            }
            if (string.IsNullOrEmpty(this.Config.ApiVersion))
            {
                result.AppendLine("Undefined ApiVersion");
            }
            if (string.IsNullOrEmpty(this.Config.Host))
            {
                result.AppendLine("Undefined Host");
            }
            if (string.IsNullOrEmpty(this.Config.Name))
            {
                result.AppendLine("Undefined Name");
            }
            if (string.IsNullOrEmpty(this.Config.User))
            {
                result.AppendLine("Undefined User");
            }

            if (result.Length > 0)
            {
                throw new InvalidOperationException($"Incorrect the state of the bus configuration: {Environment.NewLine}{result.ToString()}");
            }
        }

        public IBusConfig Config => this._config;

        public IDispatcher CreateDispatcher(IMessageHandlerResolver handlerResolver)
        {
            return new Dispatcher(this._config, this._amqpConnectionFactory, handlerResolver, this._messagePacker, this._logger);
        }

        public IPublisher CreatePublisher()
        {
            return new Publisher(this._config, this._bufferProcessing, this._amqpPublisher, this._messagePacker, this._logger);
        }

        public void Dispose()
        {
            if (_bufferProcessing != null)
            {
                _bufferProcessing.Stop();
            }
            _amqpPublisher.Dispose();
        }
    }
}
