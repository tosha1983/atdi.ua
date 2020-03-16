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
    public class EmittingParameters
    {
        [DataMember]
        public double RollOffFactor { get; set; } // from 0.85 to 1.35

        [DataMember]
        public double StandardBW { get; set; } // or channel BW
    }
}
