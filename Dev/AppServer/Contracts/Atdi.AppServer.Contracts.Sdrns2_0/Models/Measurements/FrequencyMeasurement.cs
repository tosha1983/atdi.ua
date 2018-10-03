using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Represents frequency of measurement 
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class FrequencyMeasurement
    {
        /// <summary>
        /// id
        /// </summary>
        [DataMember]
        public int Id;
        /// <summary>
        /// Frequency, Mhz
        /// </summary>
        [DataMember]
        public double Freq; 
    }
}
