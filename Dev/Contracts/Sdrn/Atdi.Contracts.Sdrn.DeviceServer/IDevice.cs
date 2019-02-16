using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface IDevice
    {
        DeviceState State { get;  }

        bool TryPushCommand(ICommandDescriptor descriptor);

        bool CheckAbilityToExecute(ICommandDescriptor descriptor);

        Type AdapterType { get;  }
    }
}
