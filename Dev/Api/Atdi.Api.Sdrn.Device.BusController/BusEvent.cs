using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal class BusEvent : IBusEvent
    {
        internal BusEvent()
        {
            this.Id = Guid.NewGuid();
            this.Created = DateTime.Now;
            this.ManagedThread = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        internal BusEvent(Guid id, DateTime created, int managedThread)
        {
            this.Id = id;
            this.Created = created;
            this.ManagedThread = managedThread;
        }

        public Guid Id { get; private set; }

        public int Code { get; set; }

        public DateTime Created { get; private set; }

        public BusEventLevel Level { get; set; }

        public string Context { get; set; }

        public string Text { get; set; }

        public int ManagedThread { get; private set; }

        public string Source { get; set; }
    }

}
