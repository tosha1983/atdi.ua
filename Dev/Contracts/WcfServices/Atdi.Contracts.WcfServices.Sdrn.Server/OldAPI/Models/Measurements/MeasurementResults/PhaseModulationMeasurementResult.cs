﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    /// <summary>
    /// Result of measurement the phas. modulation 
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class PhaseModulationMeasurementResult : MeasurementResult
    {
        /// <summary>
        /// Value
        /// </summary>
        [DataMember]
        public double? Value;
        /// <summary>
        /// StdDev
        /// </summary>
        [DataMember]
        public double? StdDev;
        /// <summary>
        /// VMin
        /// </summary>
        [DataMember]
        public double? VMin;
        /// <summary>
        /// VMmax
        /// </summary>
        [DataMember]
        public double? VMmax;
        /// <summary>
        /// Limit
        /// </summary>
        [DataMember]
        public double? Limit;
        /// <summary>
        /// Occupancy
        /// </summary>
        [DataMember]
        public double? Occupancy;
    }
}
