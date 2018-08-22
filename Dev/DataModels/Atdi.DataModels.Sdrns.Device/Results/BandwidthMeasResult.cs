using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Содержит результаты измерение ширины спектра сигнала
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class BandwidthMeasResult
    {
        /// <summary>
        /// индекс Т1 начало спектра сигнала 
        /// </summary>
        [DataMember]
        public int? T1 { get; set; }

        /// <summary>
        /// индекс Т2 конец спектра сигнала 
        /// </summary>
        [DataMember]
        public int? T2 { get; set; }
        /// <summary>
        /// индекс спектра сигнала с максимальным уровнем 
        /// </summary>
        [DataMember]
        public int? MarkerIndex { get; set; }
        /// <summary>
        /// ширина спектра в килогерцах
        /// </summary>
        [DataMember]
        public double? Bandwidth_kHz { get; set; }
        /// <summary>
        /// коректность проведеннного измерения согласно в ITU 443 
        /// </summary>
        [DataMember]
        public bool? СorrectnessEstimations { get; set; }
        /// <summary>
        /// Количество сканирований для определения BW
        /// </summary>
        [DataMember]
        public int TraceCount { get; set; }
    }
}
