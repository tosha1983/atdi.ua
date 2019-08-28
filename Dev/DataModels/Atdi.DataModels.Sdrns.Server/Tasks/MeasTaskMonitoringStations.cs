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
    /// MonitoringStation
    /// </summary>
    [Serializable]
    [DataContract(Namespace = Specification.Namespace)]
    public class MeasTaskMonitoringStations : MeasTask
    {
        /// <summary>
        /// Station Data For Measurements
        /// </summary>
        [DataMember]
        public StationDataForMeasurements[] StationsForMeasurements;
    }
}
