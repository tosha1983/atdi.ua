﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    /// <summary>
    /// Route during measurements
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class Route
    {
        /// <summary>
        /// Route identifier
        /// </summary>
        [DataMember]
        public string RouteId { get; set; }
        /// <summary>
        /// The points of route
        /// </summary>
        [DataMember]
        public RoutePoint[] RoutePoints { get; set; }
    }
}
