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
    /// Represents parameters of location for measurements.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class MeasLocParam
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public MeasLocParamIdentifier Id;
        /// <summary>
        /// Longitude, DEC
        /// </summary>
        [DataMember]
        public double? Lon;
        /// <summary>
        /// Latitude, DEC
        /// </summary>
        [DataMember]
        public double? Lat;
        /// <summary>
        /// Altitude above sea level, m
        /// </summary>
        [DataMember]
        public double? ASL;
        /// <summary>
        /// MaxDist, km
        /// </summary>
        [DataMember]
        public double? MaxDist;
    }
}
