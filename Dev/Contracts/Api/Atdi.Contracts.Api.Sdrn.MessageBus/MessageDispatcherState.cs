using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.Sdrn.MessageBus
{
    [DataContract]
    
    public enum MessageDispatcherState
    {
        [EnumMember]
        Activated = 0,
        [EnumMember]
        Deactivated = 1,
    }
}
