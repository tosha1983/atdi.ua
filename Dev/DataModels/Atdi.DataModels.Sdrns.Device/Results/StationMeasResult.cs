using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Результат измерения станции
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
        /// Глобал сид из задачи
        /// </summary>
        [DataMember]
        public string TaskGlobalSid { get; set; }

        /// <summary>
        /// измеренный глобал сид
        /// </summary>
        [DataMember]
        public string RealGlobalSid { get; set; }

        /// <summary>
        /// Идентификатор сектора
        /// </summary>
        [DataMember]
        public string SectorId { get; set; }

        /// <summary>
        /// Статус определяет соответсвие измеренных параметров лицензионным
        /// </summary>
        [DataMember]
        public string Status { get; set; }
        
        /// <summary>
        /// радиостандарт станции
        /// </summary>
        [DataMember]
        public string Standard { get; set; }


        /// <summary>
        /// Набор уровней сиигнала от даного сектора данной станции по маршруту следования
        /// </summary>
        [DataMember]
        public LevelMeasResult[] LevelResults { get; set; }

        /// <summary>
        /// обшие результаты измерения станции
        /// </summary>
        [DataMember]
        public GeneralMeasResult GeneralResult { get; set; }

    }
}
