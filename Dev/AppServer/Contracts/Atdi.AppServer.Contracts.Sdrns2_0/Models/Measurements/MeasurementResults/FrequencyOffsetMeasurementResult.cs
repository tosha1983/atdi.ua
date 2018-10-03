using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Result of measurement the frequency offset 
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class FrequencyOffsetMeasurementResult : MeasurementResult
    {
        /// <summary>
        /// Value, Hz
        /// </summary>
        [DataMember]
        public double? Value; 
        /// <summary>
        /// StdDev, Hz
        /// </summary>
        [DataMember]
        public double? StdDev;
        /// <summary>
        /// VMin, Hz
        /// </summary>
        [DataMember]
        public double? VMin;
        /// <summary>
        /// VMmax, Hz
        /// </summary>
        [DataMember]
        public double? VMmax;
        /// <summary>
        /// Limit, Hz
        /// </summary>
        [DataMember]
        public double? Limit;
        /// <summary>
        /// Occupancy, %
        /// </summary>
        [DataMember]
        public double? Occupancy;
    }
}
