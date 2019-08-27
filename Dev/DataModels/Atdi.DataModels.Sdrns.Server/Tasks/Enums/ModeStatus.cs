using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;


namespace Atdi.DataModels.Sdrns.Server
{
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public enum ModeStatus
    {
        [EnumMember]
        final,
        [EnumMember]
        cur
    }
}
