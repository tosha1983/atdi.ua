using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class MeasuredStation
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор в ICSM
        /// </summary>
        [DataMember]
        public int? IcsmSid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string GlobalSid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public StationOwner Owner { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public StationSite Site { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public StationSector[] Sectors { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Standart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public StationLicenseInfo License { get; set; }
    }
}
