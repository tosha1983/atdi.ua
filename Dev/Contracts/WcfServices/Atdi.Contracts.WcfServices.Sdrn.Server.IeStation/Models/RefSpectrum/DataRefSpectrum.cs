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
        public long? Id;

        /// <summary>
        /// Порядковый номер записи в файле csv
        /// </summary>
        [DataMember]
        public int IdNum;

        /// <summary>
        /// Наименование таблицы MOB_STATION  или MOB_STATION2
        /// </summary>
        [DataMember]
        public string TableName;

        /// <summary>
        /// Идентификатор таблицы MOB_STATION  или MOB_STATION2
        /// </summary>
        [DataMember]
        public int TableId;

        /// <summary>
        /// Идентификатор сенсора 
        /// </summary>
        [DataMember]
        public long SensorId;

        /// <summary>
        /// Строка содержащая идентификатор станции в файле csv
        /// </summary>
        [DataMember]
        public string GlobalSID;

        /// <summary>
        /// Частота
        /// </summary>
        [DataMember]
        public double Freq_MHz;

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double Level_dBm;

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? DispersionLow;

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? DispersionUp;

        /// <summary>
        /// Процент
        /// </summary>
        [DataMember]
        public double? Percent;

        /// <summary>
        /// дата
        /// </summary>
        [DataMember]
        public DateTime DateMeas;

        /// <summary>
        /// Идентификатор записи в таблице заголовка
        /// </summary>
        [DataMember]
        public long? HeadId;

        /// <summary>
        /// Статус
        /// </summary>
        [DataMember]
        public string StatusMeas;
        
    }
}
