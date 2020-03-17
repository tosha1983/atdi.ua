using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server.IeStation
{
    [DataContract(Namespace = Specification.Namespace)]
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
        E
    }
}
