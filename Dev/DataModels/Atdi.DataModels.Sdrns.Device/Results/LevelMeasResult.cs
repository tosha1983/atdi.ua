using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Содержит результат измерения уровня сигнала в одной точки от некого передатчика.
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
        public double? Level_dBm { get; set; }

        /// <summary>
        /// уровень измеренного сигнала в полосе канала 
        /// </summary>
        [DataMember]
        public double? Level_dBmkVm { get; set; }

        /// <summary>
        /// время когда был получен результат 
        /// </summary>
        [DataMember]
        public DateTime MeasurementTime { get; set; }

        /// <summary>
        /// наносекунды 10^-9 Разсинхронизация с GPS
        /// </summary>
        [DataMember]
        public double? DifferenceTimestamp_ns { get; set; }
    }
}
