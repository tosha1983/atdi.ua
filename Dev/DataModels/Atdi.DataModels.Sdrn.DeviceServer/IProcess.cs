using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public interface IProcess
    {
        Guid Id { get; }

        string Name { get; }

        long TimeStamp { get; set; }

        DateTimeOffset Date { get; }

        IProcess Parent { get; }
    }

    public abstract class ProcessBase : IProcess
    {
        public ProcessBase(string name, IProcess parentProcess = null)
        {
            this.Name = name;
            this.Id = Guid.NewGuid();
            this.Date = DateTimeOffset.Now;
            this.Parent = parentProcess;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public long TimeStamp { get; set; }

        public DateTimeOffset Date { get; private set; }

        public IProcess Parent { get; set; }
    }

    public class DeviceServerBackgroundProcess : ProcessBase
    {
        public DeviceServerBackgroundProcess() 
            : base("SDRN Device Server background process")
        {
        }
    }
}
