using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    [DataContract]
    public enum TraceEventType
    {
        [EnumMember]
        BeginScope = 0,
        [EnumMember]
        Trace,
        [EnumMember]
        EndScope
    }
}
