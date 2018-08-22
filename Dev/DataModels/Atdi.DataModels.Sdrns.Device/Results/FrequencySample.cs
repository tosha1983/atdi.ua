using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Содержит результаты измерений уровня сигнала на определенной частоте
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
        public float Freq_MHz { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public float Level_dBm { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public float Level_dBmkVm { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public float LevelMin_dBm { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public float LevelMax_dBm { get; set; }

        /// <summary>
        /// в процентах степень занятости канала
        /// </summary>
        [DataMember]
        public float Occupation_Pt { get; set; }
    }
}
