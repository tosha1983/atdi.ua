using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device.Sensors
{
    /// <summary>
    /// Represents sensor for measurement. Includes administrative and technical data.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class SensorRegistrationResult
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public string SensorId { get; set; }

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
        public string Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Message { get; set; }
    }
}
