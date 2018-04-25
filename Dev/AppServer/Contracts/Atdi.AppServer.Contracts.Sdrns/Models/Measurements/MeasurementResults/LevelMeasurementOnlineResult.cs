using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Result of measurement the level of emission  for online meas
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class LevelMeasurementOnlineResult : MeasurementResult
    {
        /// <summary>
        /// Value, dBmkV/m
        /// </summary>
        [DataMember]
        public double Value;
    }
}
