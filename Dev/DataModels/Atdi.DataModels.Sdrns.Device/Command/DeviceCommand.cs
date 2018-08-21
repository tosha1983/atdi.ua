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
    public class DeviceCommand
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string CommandId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Command { get; set; }

        /// <summary>
        /// The instance name of the SDRN Server that puts the task on measurement
        /// </summary>
        [DataMember]
        public string SdrnServer { get; set; }

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
