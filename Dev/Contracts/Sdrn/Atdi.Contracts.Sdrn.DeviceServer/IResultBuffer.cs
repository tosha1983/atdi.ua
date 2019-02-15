using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface IResultBuffer
    {
        ICommandDescriptor Descriptor { get; }

        void Push(ICommandResultPart resultPart);

        ICommandResultPart Take();
    }
}
