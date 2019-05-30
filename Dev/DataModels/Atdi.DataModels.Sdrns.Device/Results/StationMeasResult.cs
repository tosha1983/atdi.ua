using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Station measurement result
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class StationMeasResult
    {
        /// <summary>
        /// Station identifier
        /// </summary>
        [DataMember]
        public string StationId { get; set; }

        /// <summary>
        /// Global SID from the task
        /// </summary>
        [DataMember]
        public string TaskGlobalSid { get; set; }

        /// <summary>
        /// Global SID measured
        /// </summary>
        [DataMember]
        public string RealGlobalSid { get; set; }

        /// <summary>
        /// Sector identifier
        /// </summary>
        [DataMember]
        public string SectorId { get; set; }

        /// <summary>
        /// Status, defines an accordance between the measured parameters with the licensed ones
        /// </summary>
        [DataMember]
        public string Status { get; set; }
        
        /// <summary>
        /// Station radio standard
        /// </summary>
        [DataMember]
        public string Standard { get; set; }


        /// <summary>
        /// Set of signal levels from a given sector of a given station within the planned route
        /// </summary>
        [DataMember]
        public LevelMeasResult[] LevelResults { get; set; }

        /// <summary>
        /// Set of signals bearings with additional data 
        /// </summary>
        [DataMember]
        public DirectionFindingData[] Bearings { get; set; }

        /// <summary>
        /// General results of a station measurement
        /// </summary>
        [DataMember]
        public GeneralMeasResult GeneralResult { get; set; }


    }
}
