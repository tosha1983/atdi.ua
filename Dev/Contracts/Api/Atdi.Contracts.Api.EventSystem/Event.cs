using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.EventSystem
{
    public class Event : IEvent
    {
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

        public string Name { get; }

        public Guid Id { get; }

        public string Source { get; }

        public DateTimeOffset Created { get; }
    }
}
