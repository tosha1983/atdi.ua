using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    /// <summary>
    /// Represents Sensor for measurements.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class MeasSensor
    {
        /// <summary>
        /// SendorId
        /// </summary>
        [DataMember]
        public MeasSensorIdentifier SendorId;
    }
}
