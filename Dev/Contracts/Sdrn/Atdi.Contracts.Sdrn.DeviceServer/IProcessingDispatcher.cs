using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface IProcessingDispatcher
    {
        TProcess Start<TProcess>(IProcess parentProcess = null)
            where TProcess : IProcess, new();

        void Finish(IProcess process);
    }
}
