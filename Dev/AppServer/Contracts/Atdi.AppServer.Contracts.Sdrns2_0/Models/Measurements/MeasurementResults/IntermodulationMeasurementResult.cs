using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Result of measurement the intermodulation   
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class IntermodulationMeasurementResult : MeasurementResult
    {
        /// <summary>
        /// MdFreqI, MHz
        /// </summary>
        [DataMember]
        public double? MdFreqI;
        /// <summary>
        /// MdFreqA, MHz
        /// </summary>
        [DataMember]
        public double? MdFreqA;
        /// <summary>
        /// MdFreqB, MHz
        /// </summary>
        [DataMember]
        public double? MdFreqB;
        /// <summary>
        /// MdFreqC, MHz
        /// </summary>
        [DataMember]
        public double? MdFreqC;
        /// <summary>
        /// MdFrom, MHz
        /// </summary>
        [DataMember]
        public string MdFrom;
    }
}
