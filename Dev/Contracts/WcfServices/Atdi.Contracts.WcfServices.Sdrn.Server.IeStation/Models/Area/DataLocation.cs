using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server.IeStation
{
    /// <summary>
    ///  Data Location
    /// </summary>
    [DataContract(Namespace =Specification.Namespace)]
    public class DataLocation
    {
        /// <summary>
        /// Longitude, DEC
        /// </summary>
        [DataMember]
        public double Longitude { get; set; }

        /// <summary>
        /// Latitude,  DEC
        /// </summary>
        [DataMember]
        public double Latitude { get; set; }

    }
}
