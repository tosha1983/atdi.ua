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
    public class SensorDescriptor
    {
        /// <summary>
        /// Name of sensor
        /// </summary>
        [DataMember]
        public string SensorName { get; set; }

        /// <summary>
        /// Technical ID
        /// </summary>
        [DataMember]
        public string EquipmentTechId { get; set; }
    }
}
