using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Station parameters to be measured
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class MeasuredStation
    {
        /// <summary>
        /// Station identifier
        /// </summary>
        [DataMember]
        public string StationId { get; set; }

        /// <summary>
        /// Global SID
        /// </summary>
        [DataMember]
        public string GlobalSid { get; set; }
        
        /// <summary>
        /// Owner global SID
        /// </summary>
        [DataMember]
        public string OwnerGlobalSid { get; set; }

        /// <summary>
        /// Station owner
        /// </summary>
        [DataMember]
        public StationOwner Owner { get; set; }

        /// <summary>
        /// Station site
        /// </summary>
        [DataMember]
        public StationSite Site { get; set; }

        /// <summary>
        /// Station sectors
        /// </summary>
        [DataMember]
        public StationSector[] Sectors { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Standard
        /// </summary>
        [DataMember]
        public string Standard { get; set; }

        /// <summary>
        /// Station license information
        /// </summary>
        [DataMember]
        public StationLicenseInfo License { get; set; }
    }
}
