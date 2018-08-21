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
    public class MeasResults
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string TaskId { get; set; }

        /// <summary>
        /// конкретное время окончания измерения  // для мобильных измерительных комплексов это время когда отправляется результат.
        /// </summary>
        [DataMember]
        public DateTime Measured { get; set; }

        /// <summary>
        /// статус обекта
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Используется для SO и храниться для запоминания количества измерений
        /// </summary>
        [DataMember]
        public int ScansSONumber { get; set; }

        /// <summary>
        /// Number of scans at a time.
        /// </summary>
        [DataMember]
        public int SwNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public GeoLocation Location { get; set; }

        /// <summary>
        /// тип измерения 
        /// </summary>
        [DataMember]
        public MeasurementType Measurement { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public FrequencySample[] FrequencySamples { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public float[] Frequencies { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public float[] Levels { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public StationMeasResult[] StationResults { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public BandwidthMeasResult BandwidthResult { get; set; }
    }
}
