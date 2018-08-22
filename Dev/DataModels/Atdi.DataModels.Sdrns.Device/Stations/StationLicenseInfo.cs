using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// данные дозвола
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class StationLicenseInfo
    {
        /// <summary>
        /// из ICSM
        /// </summary>
        [DataMember]
        public int? IcsmId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// дата закртытия 
        /// </summary>
        [DataMember]
        public DateTime? CloseDate { get; set; }

    }
}
