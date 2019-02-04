using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Identifies the Sensor equipment
    /// </summary>
    /// 
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class SensorDescriptor
    {
        /// <summary>
        /// Instance name of SDRN Server, which puts a measurement task
        /// </summary>
        [DataMember]
        public string SdrnServer { get; set; }

        /// <summary>
        /// Name of sensor
        /// </summary>
        [DataMember]
        public string SensorName { get; set; }

        /// <summary>
        /// Equipment technical ID
        /// </summary>
        [DataMember]
        public string EquipmentTechId { get; set; }
    }
}
