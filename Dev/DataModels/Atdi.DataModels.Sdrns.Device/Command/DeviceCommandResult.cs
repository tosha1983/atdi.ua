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
    public class DeviceCommandResult
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string CommandId { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        public string SensorName { get; set; }

        /// <summary>
        /// Technical ID
        /// </summary>
        [DataMember]
        public string EquipmentTechId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string CustTxt1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? CustData1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? CustNbr1 { get; set; }
    }
}
