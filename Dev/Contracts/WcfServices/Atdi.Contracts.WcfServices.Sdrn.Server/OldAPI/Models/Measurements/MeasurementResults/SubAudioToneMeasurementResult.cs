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
    /// Result of measurement the SubAudioTone  
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class SubAudioToneMeasurementResult : MeasurementResult
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
        /// Occupancy, Hz
        /// </summary>
        [DataMember]
        public double? Occupancy;
    }
}
