using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface IAdapterHost
    {
        void RegisterHandler<TCommand, TResult>(Action<TCommand, IExecutionContext> commandHandler)
            where TCommand : new();
    }
}
