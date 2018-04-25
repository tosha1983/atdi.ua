using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Result of measurement the level of emission 
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class LevelMeasurementResult : MeasurementResult
    {
        /// <summary>
        /// Value, dBmkV/m
        /// </summary>
        [DataMember]
        public double? Value;
        /// <summary>
        /// StdDev, dBmkV/m
        /// </summary>
        [DataMember]
        public double? StdDev;
        /// <summary>
        /// VMin, dBmkV/m
        /// </summary>
        [DataMember]
        public double? VMin;
        /// <summary>
        /// VMmax, dBmkV/m
        /// </summary>
        [DataMember]
        public double? VMmax;
        /// <summary>
        /// Limit, dBmkV/m
        /// </summary>
        [DataMember]
        public double? Limit;
        /// <summary>
        /// Occupancy, %
        /// </summary>
        [DataMember]
        public double? Occupancy;
        /// <summary>
        /// PMin, dBmkV/m
        /// </summary>
        [DataMember]
        public double? PMin;
        /// <summary>
        /// PMax, dBmkV/m
        /// </summary>
        [DataMember]
        public double? PMax;
        /// <summary>
        /// PDiff, dBmkV/m
        /// </summary>
        [DataMember]
        public double? PDiff;
    }
}
