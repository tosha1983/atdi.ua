using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.Server
{
    public interface ISdrnReceivedMessage<TObject> : IReceivedMessage<TObject>
    {
        string DeviceSensorName { get; }

        string DeviceSensorTechId { get; }
    }
}
