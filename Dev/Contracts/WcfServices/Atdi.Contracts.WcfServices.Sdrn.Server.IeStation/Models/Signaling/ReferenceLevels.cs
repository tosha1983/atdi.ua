using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;


namespace Atdi.Contracts.WcfServices.Sdrn.Server.IeStation
{
    /// <summary>
    /// Represent triggers levels adopted to trace of Devise (SDR)
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class ReferenceLevels
    {
        [DataMember]
        public double StartFrequency_Hz { get; set; }

        [DataMember]
        public double StepFrequency_Hz { get; set; }

        [DataMember]
        public float[] levels { get; set; }
    }
}
