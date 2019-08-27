using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.DataModels.Sdrns.Server
{
    /// <summary>
    /// Represents Sensor for measurements.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class MeasSensor
    {
        /// <summary>
        /// SensorId
        /// </summary>
        [DataMember]
        public MeasSensorIdentifier SensorId;
        /// <summary>
        /// Sensor name
        /// </summary>
        [DataMember]
        public string SensorName;
        /// <summary>
        /// Sensor TechId
        /// </summary>
        [DataMember]
        public string SensorTechId;
    }
}
