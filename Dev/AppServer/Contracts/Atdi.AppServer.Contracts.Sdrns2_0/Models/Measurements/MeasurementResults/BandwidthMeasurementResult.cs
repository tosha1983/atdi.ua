using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Result of measurement the bandwidth
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class BandwidthMeasurementResult : MeasurementResult
    {
        /// <summary>
        /// Value, kHz
        /// </summary>
        [DataMember]
        public double? Value;
        /// <summary>
        /// StdDev, kHz
        /// </summary>
        [DataMember]
        public double? StdDev;
        /// <summary>
        /// VMin, kHz
        /// </summary>
        [DataMember]
        public double? VMin;
        /// <summary>
        /// VMmax, kHz
        /// </summary>
        [DataMember]
        public double? VMmax;
        /// <summary>
        /// Limit, kHz
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
