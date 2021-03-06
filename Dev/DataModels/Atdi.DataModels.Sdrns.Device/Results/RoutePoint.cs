﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Presents point of route
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class RoutePoint: GeoLocation
    {
        /// <summary>
        /// Time of the beginning of stay in this location
        /// </summary>
        [DataMember]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Time of the end of stay in this location
        /// </summary>
        [DataMember]
        public DateTime FinishTime { get; set; }

        /// <summary>
        /// Characterizes the reason for the stay of the measuring device at a point
        /// </summary>
        [DataMember]
        public PointStayType PointStayType { get; set; }
    }
}
