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
    public class LevelMeasResult
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public GeoLocation Location { get; set; }

        /// <summary>
        /// уровень измеренного сигнала в полосе канала
        /// </summary>
        [DataMember]
        public double? LeveldBm { get; set; }

        /// <summary>
        /// уровень измеренного сигнала в полосе канала 
        /// </summary>
        [DataMember]
        public double? LeveldBmkVm { get; set; }

        /// <summary>
        /// время когда был получен результат 
        /// </summary>
        [DataMember]
        public DateTime MeasurementTime { get; set; }

        /// <summary>
        /// наносекунды 10^-9 Разсинхронизация с GPS
        /// </summary>
        [DataMember]
        public double? DifferenceTimestamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public decimal? CentralFrequency { get; set; }

        /// <summary>
        /// кГц;
        /// </summary>
        [DataMember]
        public double? BW { get; set; }

        /// <summary>
        /// кГц;
        /// </summary>
        [DataMember]
        public double? RBW { get; set; }

        /// <summary>
        /// кГц;
        /// </summary>
        [DataMember]
        public double? VBW { get; set; }

    }
}
