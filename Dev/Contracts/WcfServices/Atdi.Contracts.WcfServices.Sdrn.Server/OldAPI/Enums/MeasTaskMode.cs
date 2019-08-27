using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    [DataContract(Namespace = Specification.Namespace)]
    public enum MeasTaskMode
    {
        [EnumMember]
        Run,
        [EnumMember]
        Stop,
        [EnumMember]
        Del,
        [EnumMember]
        New
    }
}
