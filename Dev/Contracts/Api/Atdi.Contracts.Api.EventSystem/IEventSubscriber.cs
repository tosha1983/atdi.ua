using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.EventSystem
{
    public interface IEventSubscriber<TEvent>
        where TEvent : class, IEvent, new()
    {
        void Notify(TEvent @event);
    }
}
