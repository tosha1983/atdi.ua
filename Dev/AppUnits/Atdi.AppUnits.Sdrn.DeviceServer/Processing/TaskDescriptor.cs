using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    class TaskDescriptor : ITaskDescriptor
    {
        public ITaskContext Parent { get; set; }

        public CancellationToken Token { get; set; }

        public ITask Task { get; set; }

        public IProcess Process { get; set; }

        public Type TaskType { get; set; }

        public Type ProcessType { get; set; }
    }
}
