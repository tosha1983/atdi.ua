﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.EventSystem
{
    public interface IEventEmitter 
    {
        Guid Emit(IEvent @event, EventEmittingOptions options);
    }

    public static class EventEmitterExtension
    {
        public static Guid Emit(this IEventEmitter emitter, string eventName, string source = null)
        {
            var options = new EventEmittingOptions
            {
                Rule = EventEmittingRule.Default
            };

            var @event = new Event(eventName, source);
            return emitter.Emit(@event, options);
        }
    }
}