using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Represents the sensor for measurement. Includes administrative and technical data
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class SensorUpdatingResult
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
        /// Technical ID
        /// </summary>
        [DataMember]
        public string EquipmentTechId { get; set; }

        /// <summary>
        /// Sensor status
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        [DataMember]
        public string Message { get; set; }
    }
}
