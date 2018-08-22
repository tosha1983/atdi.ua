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
    public class FrequencySample
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// MHz
        /// </summary>
        [DataMember]
        public float Freq { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public float LeveldBm { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public float LeveldBmkVm { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public float LevelMindBm { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public float LevelMaxdBm { get; set; }

        /// <summary>
        /// в процентах степень занятости канала
        /// </summary>
        [DataMember]
        public float OccupationPt { get; set; }
    }
}
