using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface ICommandsHost
    {
        void Register(IDevice device, CommandType commandType, Type instanceType);

        IDevice[] GetDevices(Type instanceType);

        IDevice[] GetDevices(CommandType commandType);
    }
}
