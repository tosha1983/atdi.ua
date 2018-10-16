using Atdi.Modules.Sdrn.AmqpBroker;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.BusController
{
    public class SdrnBrokerObserver : LoggedObject, IBrokerObserver
    {
        public SdrnBrokerObserver(ILogger logger) : base(logger)
        {
        }

        public void OnEvent(IBrokerEvent brokerEvent)
        {
            if (brokerEvent.Level == BrokerEventLevel.Critical)
            {
                this.Logger.Critical((EventContext)brokerEvent.Context, (EventText)brokerEvent.Text, brokerEvent.Source);
            }
            else if (brokerEvent.Level == BrokerEventLevel.Debug)
            {
                this.Logger.Debug((EventContext)brokerEvent.Context, (EventText)brokerEvent.Text, brokerEvent.Source);
            }
            else if (brokerEvent.Level == BrokerEventLevel.Error)
            {
                this.Logger.Error((EventContext)brokerEvent.Context, (EventText)brokerEvent.Text, brokerEvent.Source);
            }
            else if (brokerEvent.Level == BrokerEventLevel.Exception)
            {
                this.Logger.Exception((EventContext)brokerEvent.Context, brokerEvent.Exception, brokerEvent.Source);
            }
            else if (brokerEvent.Level == BrokerEventLevel.Info)
            {
                this.Logger.Info((EventContext)brokerEvent.Context, (EventText)brokerEvent.Text);
            }
            else if (brokerEvent.Level == BrokerEventLevel.Trace)
            {
                this.Logger.Debug((EventContext)brokerEvent.Context, (EventText)brokerEvent.Text, brokerEvent.Source);
            }
            else if (brokerEvent.Level == BrokerEventLevel.Verbouse)
            {
                this.Logger.Verbouse((EventContext)brokerEvent.Context, (EventText)brokerEvent.Text);
            }
            else if (brokerEvent.Level == BrokerEventLevel.Warning)
            {
                this.Logger.Warning((EventContext)brokerEvent.Context, (EventText)brokerEvent.Text);
            }
        }
    }
}
