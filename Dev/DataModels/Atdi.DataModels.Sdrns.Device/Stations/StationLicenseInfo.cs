using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// License information
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class StationLicenseInfo
    {
        /// <summary>
        /// License ID from ICSM
        /// </summary>
        [DataMember]
        public int? IcsmId { get; set; }

        /// <summary>
        /// License name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Start date
        /// </summary>
        [DataMember]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End date
        /// </summary>
        [DataMember]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Close date
        /// </summary>
        [DataMember]
        public DateTime? CloseDate { get; set; }

    }
}
