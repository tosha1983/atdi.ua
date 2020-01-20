using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    

    public interface IController : IDisposable
    {
        void SendCommand<TResult>(ITaskContext taskContext, ICommand command, CancellationToken cancellationToken, ControllerFailureAction onFailureAction);

        TProperties[] EnsureDevicesProperties<TProperties>(CommandType commandType)
            where TProperties : IDeviceProperties, new();

        IReadOnlyDictionary<CommandType, IDeviceProperties[]> GetDevicesProperties();
            
    }
}
