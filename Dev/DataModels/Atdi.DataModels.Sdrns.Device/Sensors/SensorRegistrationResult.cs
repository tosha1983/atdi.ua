using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device.Sensors
{
    /// <summary>
    /// Represents the sensor for measurement. Includes administrative and technical data
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class SensorRegistrationResult
    {
        /// <summary>
        /// Sensor identifier
        /// </summary>
        [DataMember]
        public string SensorId { get; set; }

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
        /// Status
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
