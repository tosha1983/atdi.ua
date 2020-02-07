using Atdi.Api.EventSystem;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Infocenter
{
    public class EventSystemObserver : LoggedObject, IEventSystemObserver
    {
        public EventSystemObserver(ILogger logger) : base(logger)
        {
        }

        public void OnEvent(IEventSystemEvent @event)
        {
            if (@event.Level == EventSystemEventLevel.Critical)
            {
                this.Logger.Critical((EventContext)@event.Context, (EventText)@event.Text, @event.Source);
            }
            else if (@event.Level == EventSystemEventLevel.Debug)
            {
                this.Logger.Debug((EventContext)@event.Context, (EventText)@event.Text, @event.Source);
            }
            else if (@event.Level == EventSystemEventLevel.Error)
            {
                this.Logger.Error((EventContext)@event.Context, (EventText)@event.Text, @event.Source);
            }
            else if (@event.Level == EventSystemEventLevel.Exception)
            {
                this.Logger.Exception((EventContext)@event.Context, @event.Exception, @event.Source);
            }
            else if (@event.Level == EventSystemEventLevel.Info)
            {
                this.Logger.Info((EventContext)@event.Context, (EventText)@event.Text);
            }
            else if (@event.Level == EventSystemEventLevel.Trace)
            {
                this.Logger.Debug((EventContext)@event.Context, (EventText)@event.Text, @event.Source);
            }
            else if (@event.Level == EventSystemEventLevel.Verbouse)
            {
                this.Logger.Verbouse((EventContext)@event.Context, (EventText)@event.Text);
            }
            else if (@event.Level == EventSystemEventLevel.Warning)
            {
                this.Logger.Warning((EventContext)@event.Context, (EventText)@event.Text);
            }
        }
    }
}
