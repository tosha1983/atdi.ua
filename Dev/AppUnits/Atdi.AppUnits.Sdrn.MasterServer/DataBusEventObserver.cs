using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Api.DataBus;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.MasterServer
{
    internal sealed class DataBusEventObserver : LoggedObject, IBusEventObserver
    {
        public DataBusEventObserver(ILogger logger) : base(logger)
        {
        }

        public void OnEvent(IBusEvent busEvent)
        {
            if (busEvent.Level == BusEventLevel.Critical)
            {
                this.Logger.Critical((EventContext)busEvent.Context, (EventText)busEvent.Text, busEvent.Source);
            }
            else if (busEvent.Level == BusEventLevel.Debug)
            {
                this.Logger.Debug((EventContext)busEvent.Context, (EventText)busEvent.Text, busEvent.Source);
            }
            else if (busEvent.Level == BusEventLevel.Error)
            {
                this.Logger.Error((EventContext)busEvent.Context, (EventText)busEvent.Text, busEvent.Source);
            }
            else if (busEvent.Level == BusEventLevel.Exception)
            {
                this.Logger.Exception((EventContext)busEvent.Context, busEvent.Exception, busEvent.Source);
            }
            else if (busEvent.Level == BusEventLevel.Info)
            {
                this.Logger.Info((EventContext)busEvent.Context, (EventText)busEvent.Text);
            }
            else if (busEvent.Level == BusEventLevel.Trace)
            {
                this.Logger.Debug((EventContext)busEvent.Context, (EventText)busEvent.Text, busEvent.Source);
            }
            else if (busEvent.Level == BusEventLevel.Verbouse)
            {
                this.Logger.Verbouse((EventContext)busEvent.Context, (EventText)busEvent.Text);
            }
            else if (busEvent.Level == BusEventLevel.Warning)
            {
                this.Logger.Warning((EventContext)busEvent.Context, (EventText)busEvent.Text);
            }
        }
    }
}
