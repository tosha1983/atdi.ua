using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.DataModels.Sdrns.Server;
using System.Runtime.Serialization;

namespace Atdi.DataModels.Sdrns.Server
{
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public enum Status
    {
        [EnumMember]
        A,
        [EnumMember]
        N,
        [EnumMember]
        F,
        [EnumMember]
        Z,
        [EnumMember]
        O,
        [EnumMember]
        P,
        [EnumMember]
        E_L,
        [EnumMember]
        C,
        [EnumMember]
        T,
        [EnumMember]
        E_E,
        [EnumMember]
        E_T,
        [EnumMember]
        E,
        [EnumMember]
        S
    }
}
