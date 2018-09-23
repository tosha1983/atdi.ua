using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Modules.Sdrn.MessageBus;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal sealed class BusGate : IBusGate
    {
        private readonly BusLogger _logger;
        private readonly EnvironmentDescriptor _environmentDescriptor;
        private readonly MessageConverter _messageConverter;

        internal BusGate(EnvironmentDescriptor environmentDescriptor, MessageConverter messageConverter, BusLogger logger)
        {
            this._logger = logger;
            this._environmentDescriptor = environmentDescriptor;
            this._messageConverter = messageConverter;
        }

        public IBusGateConfig Config => this._environmentDescriptor.GateConfig;

        public IMessageDispatcher CreateDispatcher(string name, IBusEventObserver eventObserver = null)
        {
            try
            {
                BusLogger logger = _logger;
                if (eventObserver != null)
                {
                    logger = new BusLogger(eventObserver);
                }

                var dispatcher = new MessageDispatcher(name, this._environmentDescriptor, this._messageConverter, logger);
                logger.Info(0, "CreateDispatcher", "The object of the dispatcher was created saccessfully", this);

                return dispatcher;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("The object of the dispatcher was not created", e);
            }
        }

        public IMessagePublisher CreatePublisher(IBusEventObserver eventObserver = null)
        {
            try
            {
                BusLogger logger = _logger;
                if (eventObserver != null)
                {
                    logger = new BusLogger(eventObserver);
                }

                var publisher = new MessagePublisher(this._environmentDescriptor, this._messageConverter, logger);
                logger.Info(0, "CreatePublisher", "The object of the publisher was created saccessfully", this);

                return publisher;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("The object of the publisher was not created", e);
            }
        }

        public void Dispose()
        {
        }
    }
}
