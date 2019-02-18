using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    class ProcessingDispatcher : IProcessingDispatcher
    {
        public void Finish(IProcess process)
        {
            throw new NotImplementedException();
        }

        public TProcess Start<TProcess>(IProcess parentProcess = null) where TProcess : IProcess, new()
        {
            throw new NotImplementedException();
        }
    }
}
