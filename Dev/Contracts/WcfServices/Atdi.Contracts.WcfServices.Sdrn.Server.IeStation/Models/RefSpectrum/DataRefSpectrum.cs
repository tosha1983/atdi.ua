using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server.IeStation
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace =Specification.Namespace)]
    public class DataRefSpectrum
    {
        /// <summary>
        /// Идентификатор записи в БД SDRN
        /// </summary>
        [DataMember]
        public long? Id { get; set; }

        /// <summary>
        /// Порядковый номер записи в файле csv
        /// </summary>
        [DataMember]
        public int IdNum { get; set; }

        /// <summary>
        /// Наименование таблицы MOB_STATION  или MOB_STATION2
        /// </summary>
        [DataMember]
        public string TableName { get; set; }

        /// <summary>
        /// Идентификатор таблицы MOB_STATION  или MOB_STATION2
        /// </summary>
        [DataMember]
        public int TableId { get; set; }

        /// <summary>
        /// Идентификатор сенсора 
        /// </summary>
        [DataMember]
        public long SensorId { get; set; }

        /// <summary>
        /// Строка содержащая идентификатор станции в файле csv
        /// </summary>
        [DataMember]
        public string GlobalSID { get; set; }

        /// <summary>
        /// Частота
        /// </summary>
        [DataMember]
        public double Freq_MHz { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double Level_dBm { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? DispersionLow { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? DispersionUp { get; set; }

        /// <summary>
        /// Процент
        /// </summary>
        [DataMember]
        public double? Percent { get; set; }

        /// <summary>
        /// дата
        /// </summary>
        [DataMember]
        public DateTime DateMeas { get; set; }

        /// <summary>
        /// Идентификатор записи в таблице заголовка
        /// </summary>
        [DataMember]
        public long? HeadId { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        [DataMember]
        public string StatusMeas { get; set; }

    }
}
