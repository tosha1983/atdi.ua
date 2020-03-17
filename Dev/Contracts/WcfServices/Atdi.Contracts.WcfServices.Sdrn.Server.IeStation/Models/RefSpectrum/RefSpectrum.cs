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
    /// RefSpectrum
    /// </summary>
    [DataContract(Namespace =Specification.Namespace)]
    public class RefSpectrum
    {
        
        /// <summary>
        /// Идентификатор записи в SDRN базе 
        /// </summary>
        [DataMember]
        public long? Id { get; set; }
        /// <summary>
        /// Наименование csv - файла
        /// </summary>
        [DataMember]
        public string FileName { get; set; }

        /// <summary>
        /// Дата загрузки файла
        /// </summary>
        [DataMember]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Имя пользователя, который выполнил загрузку
        /// </summary>
        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public int? CountImportRecords { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? MinFreqMHz { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? MaxFreqMHz { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? CountSensors { get; set; }

        /// <summary>
        /// Массив данных DataRefSpectrum (собственно наполнение csv - файла)
        /// </summary>
        [DataMember]
        public DataRefSpectrum[] DataRefSpectrum { get; set; }

    }
}
