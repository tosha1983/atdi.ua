
using Atdi.Modules.AmqpBroker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EventSystem
{
    public class AmqpBrokerObserver : IBrokerObserver
    {
        private readonly IEventSystemObserver _logger;

        public AmqpBrokerObserver(IEventSystemObserver logger) 
        {
            this._logger = logger;
        }

        public void OnEvent(IBrokerEvent brokerEvent)
        {
            if (brokerEvent.Level == BrokerEventLevel.Critical)
            {
                this._logger.Critical(brokerEvent.Code, brokerEvent.Context, brokerEvent.Text, brokerEvent.Source);
            }
            else if (brokerEvent.Level == BrokerEventLevel.Debug)
            {
                this._logger.Debug(brokerEvent.Code, brokerEvent.Context, brokerEvent.Text, brokerEvent.Source);
            }
            else if (brokerEvent.Level == BrokerEventLevel.Error)
            {
                this._logger.Error(brokerEvent.Code, brokerEvent.Context, brokerEvent.Text, brokerEvent.Source);
            }
            else if (brokerEvent.Level == BrokerEventLevel.Exception)
            {
                this._logger.Exception(brokerEvent.Code, brokerEvent.Context, brokerEvent.Exception, brokerEvent.Source);
            }
            else if (brokerEvent.Level == BrokerEventLevel.Info)
            {
                this._logger.Info(brokerEvent.Code, brokerEvent.Context, brokerEvent.Text, brokerEvent.Source);
            }
            else if (brokerEvent.Level == BrokerEventLevel.Trace)
            {
                this._logger.Trace(brokerEvent.Code, brokerEvent.Context, brokerEvent.Text, brokerEvent.Source);
            }
            else if (brokerEvent.Level == BrokerEventLevel.Verbouse)
            {
                this._logger.Verbouse(brokerEvent.Context, brokerEvent.Text, brokerEvent.Source);
            }
            else if (brokerEvent.Level == BrokerEventLevel.Warning)
            {
                this._logger.Warning(brokerEvent.Code, brokerEvent.Context, brokerEvent.Text, brokerEvent.Source);
            }
        }
    }
}
