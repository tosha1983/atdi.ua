﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server.IeStation
{
    /// <summary>
    /// Represents location of sensor
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class SensorLocation
    {
        /// <summary>
        /// DataFrom
        /// </summary>
        [DataMember]
        public DateTime? DataFrom;
        /// <summary>
        /// DataTo
        /// </summary>
        [DataMember]
        public DateTime? DataTo;
        /// <summary>
        /// DataCreated
        /// </summary>
        [DataMember]
        public DateTime? DataCreated;
        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        public string Status;
        /// <summary>
        /// Longitude, DEC
        /// </summary>
        [DataMember]
        public Double? Lon; 
        /// <summary>
        /// Latitude,  DEC
        /// </summary>
        [DataMember]
        public Double? Lat;
        /// <summary>
        /// Altitude above sea level, m
        /// </summary>
        [DataMember]
        public Double? ASL;
    }
}
