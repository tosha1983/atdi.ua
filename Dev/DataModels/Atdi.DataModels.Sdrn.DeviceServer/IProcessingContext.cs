using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public interface IProcessingContext
    {
        Guid Id { get; }

        string Name { get; }

        string Description { get; }

        long TimeStamp { get; }

        DateTimeOffset Date { get; }
    }

    public abstract class ProcessingContextBase : IProcessingContext
    {
        public ProcessingContextBase(long timeStamp = 0,  string name = null, string description = null)
        {
            this.TimeStamp = timeStamp;
            this.Name = name;
            this.Description = description;
            this.Id = Guid.NewGuid();
            this.Date = DateTimeOffset.Now;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public long TimeStamp { get; private set; }

        public DateTimeOffset Date { get; private set; }

    }
}
