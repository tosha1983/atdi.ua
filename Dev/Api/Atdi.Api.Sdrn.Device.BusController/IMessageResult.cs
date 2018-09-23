using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal interface IMessageResult
    {
        MessageHandlingResult Result { get; set; }

        string ReasonFailure { get; set; }
    }
}
