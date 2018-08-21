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
    public class StationMeasResult
    {
        /// <summary>
        /// Идентификатор станции
        /// </summary>
        [DataMember]
        public string StationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string TaskGlobalSid { get; set; }

        /// <summary>
        /// измеренный
        /// </summary>
        [DataMember]
        public string RealGlobalSid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string SectorId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public LevelMeasResult[] LevelResults { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public GeneralMeasResult GeneralResult { get; set; }
    }
}
