using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface IResultBuffer : IDisposable
    {
        Guid Id { get; }

        void Push(ICommandResultPart resultPart);

        bool TryTake(out ICommandResultPart result, CancellationToken token);

        void Cancel();
    }
}
