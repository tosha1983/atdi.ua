using Atdi.Contracts.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EventSystem
{
    public class EventEmitter : IEventEmitter
    {
        private readonly EventSite _eventSite;

        public EventEmitter(IEventSite eventSite)
        {
            this._eventSite = eventSite as EventSite;
        }

        public void Dispose()
        {

        }

        public Guid Emit(IEvent @event, EventEmittingOptions options)
        {
            return Guid.NewGuid();
        }
    }
}
