using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    /// <summary>
    /// Represents frequency of measurement 
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class FrequencyMeasurement
    {
        /// <summary>
        /// id
        /// </summary>
        [DataMember]
        public long Id;
        /// <summary>
        /// Frequency, Mhz
        /// </summary>
        [DataMember]
        public double Freq; 
    }
}
