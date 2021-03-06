﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Result of measurement the ampl. modulation
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class AmplitudeModulationMeasurementResult: MeasurementResult
    {
        /// <summary>
        /// Value, %
        /// </summary>
        [DataMember]
        public double? Value;
        /// <summary>
        /// StdDev, %
        /// </summary>
        [DataMember]
        public double? StdDev;
        /// <summary>
        /// VMin, %
        /// </summary>
        [DataMember]
        public double? VMin;
        /// <summary>
        /// VMmax, %
        /// </summary>
        [DataMember]
        public double? VMmax;
        /// <summary>
        /// Limit, %
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
