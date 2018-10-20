using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.EventSystem
{
    public class Event : IEvent
    {
        public Event()
        {
        }

        public Event(string name)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Created = DateTimeOffset.Now;
        }

        public Event(string name, string source) : this(name)
        {
            this.Source = source;
        }

        public string Name { get; set; }

        public Guid Id { get; set; }

        public string Source { get; set; }

        public DateTimeOffset Created { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SubscriptionEventAttribute : Attribute
    {
        public SubscriptionEventAttribute()
        {
        }

        public SubscriptionEventAttribute(string eventName)
        {
            this.EventName = eventName;
        }

        public string EventName { get; set; }

        public string SubscriberName { get; set; }

    }
}
