using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    class Event : IEvent
    {
        public Event(EventLevel level)
        {
            this.Level = level;
            this.Id = Guid.NewGuid();
            this.ManagedThread = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        public Guid Id { get; set; }

        public EventLevel Level { get; private set; }

        public DateTime Time { get; set; }

        public int OSThread { get; set; }

        public int ManagedThread { get; set; }

        public EventContext Context { get; set; }

        public EventCategory Category { get; set; }

        public EventText Text { get; set; }
    }
}
