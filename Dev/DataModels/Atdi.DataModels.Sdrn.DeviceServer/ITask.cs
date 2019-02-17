using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public interface ITask
    {
        Guid Id { get; }

        long TimeStamp { get; set; }

        DateTimeOffset Date { get; }

        TaskExecutionOption Options { get; set; }

        long Delay { get; set; }

        TaskState State { get; }
    }

    public abstract class TaskBase : ITask
    {
        public TaskBase()
        {
            this.Date = DateTimeOffset.Now;
            this.Id = Guid.NewGuid();
            this.State = TaskState.Created;
        }

        public Guid Id { get; private set; }

        public long TimeStamp { get; set; }

        public DateTimeOffset Date { get; private set; }

        public TaskExecutionOption Options { get; set; }

        public long Delay { get; set; }

        public TaskState State { get; private set; }
    }

    public abstract class AutoTask : TaskBase
    {
    }
}
