using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.Sdrn.MessageBus;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal class MessageResult : IMessageResult
    {
        public MessageHandlingResult Result { get; set; }
        public string ReasonFailure { get; set; }
    }
}
