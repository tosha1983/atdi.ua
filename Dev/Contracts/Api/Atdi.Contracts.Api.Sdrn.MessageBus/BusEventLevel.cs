using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.Sdrn.MessageBus
{
    [DataContract]
    [Flags]
    public enum BusEventLevel
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        Verbouse = 1,
        [EnumMember]
        Info = 2,
        [EnumMember]
        Warning = 4,
        [EnumMember]
        Trace = 8,
        [EnumMember]
        Debug = 16,
        [EnumMember]
        Error = 32,
        [EnumMember]
        Exception = 64,
        [EnumMember]
        Critical = 128
    }
}
