using Atdi.Contracts.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EventSystem
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly EventSite _eventSite;

        public EventDispatcher(IEventSite eventSite)
        {
            this._eventSite = eventSite as EventSite;
        }

        public void Subscribe(string eventName, Type type)
        {

        }

        public void Unsubscribe(string eventName, Type type)
        {

        }
    }
}
