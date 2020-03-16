using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server.IeStation
{
    [DataContract(Namespace = Specification.Namespace)]
    public class WorkTime
    {
        [DataMember]
        public DateTime StartEmitting { get; set; }

        [DataMember]
        public DateTime StopEmitting { get; set; }

        [DataMember]
        public int HitCount { get; set; }

        [DataMember]
        public float PersentAvailability { get; set; }
    }
}
