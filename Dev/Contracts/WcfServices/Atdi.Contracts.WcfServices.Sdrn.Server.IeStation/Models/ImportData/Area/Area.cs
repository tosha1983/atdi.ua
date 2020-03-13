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
    ///  geolocation parameters
    /// </summary>
    [DataContract(Namespace =Specification.Namespace)]
    public class Area
    {
        /// <summary>
        /// Identifier in ICSM DB
        /// </summary>
        [DataMember]
        public int IdentifierFromICSM { get; set; }

        /// <summary>
        /// Name area
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Type of the area
        /// </summary>
        [DataMember]
        public string TypeArea { get; set; }

        /// <summary>
        /// Created by
        /// </summary>
        [DataMember]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Date created
        /// </summary>
        [DataMember]
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Area Location
        /// </summary>
        [DataMember]
        public DataLocation[] Location { get; set; }
    }
}
