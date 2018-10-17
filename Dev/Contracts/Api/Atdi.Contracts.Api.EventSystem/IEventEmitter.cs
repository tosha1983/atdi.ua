using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.EventSystem
{
    public interface IEventEmitter : IDisposable
    {
        void Emit(IEvent @event, EventEmittingOptions options);
    }

    public static class EventEmitterExtension
    {
        public static void Emit(this IEventEmitter emitter, string eventName, string source = null)
        {
            var options = new EventEmittingOptions
            {
                Rule = EventEmittingRule.Default
            };

            var @event = new Event(eventName, source);
            emitter.Emit(@event, options);
        }
    }
}
