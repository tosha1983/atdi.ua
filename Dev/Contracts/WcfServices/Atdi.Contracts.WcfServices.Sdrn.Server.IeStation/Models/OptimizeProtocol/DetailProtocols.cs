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
    public class DetailProtocols
    {
        [DataMember]
        public long? Id { get; set; }
        /// <summary>
        /// Дата вимірювання
        /// </summary>
        [DataMember]
        public DateTime? DateMeas { get; set; }
        /// <summary>
        /// Дата створення 
        /// </summary>
        [DataMember]
        public DateTime? DateCreated { get; set; }
        /// <summary>
        /// Хто створив
        /// </summary>
        [DataMember]
        public string CreatedBy { get; set; }
        /// <summary>
        /// Стандарт
        /// </summary>
        [DataMember]
        public string StandardName { get; set; }
        /// <summary>
        /// Означення станції, фактично
        /// </summary>
        [DataMember]
        public string GlobalSID { get; set; }
        /// <summary>
        /// Означення станції (заявлено)
        /// </summary>
        [DataMember]
        public string PermissionGlobalSID { get; set; }
        /// <summary>
        /// Частота передачі заявлена МГц
        /// </summary>
        [DataMember]
        public string StationTxFreq { get; set; }
        /// <summary>
        /// Частота прийому заявлена МГц
        /// </summary>
        [DataMember]
        public string StationRxFreq { get; set; }
        /// <summary>
        /// Канал передачі заявлений
        /// </summary>
        [DataMember]
        public string StationTxChannel { get; set; }
        /// <summary>
        /// Канал прийому заявлений
        /// </summary>
        [DataMember]
        public string StationRxChannel { get; set; }
        /// <summary>
        /// Результат радіоконтролю
        /// </summary>
        [DataMember]
        public string StatusMeas { get; set; }
        /// <summary>
        /// Назва сенсора
        /// </summary>
        [DataMember]
        public string SensorName { get; set; }
        /// <summary>
        /// Частота, MHz з файлу RefSpectrum
        /// </summary>
        [DataMember]
        public double? Freq_MHz { get; set; }
        /// <summary>
        /// ширина смуги (згідно дозволу ) (кГц)
        /// </summary>
        [DataMember]
        public double? BandWidth { get; set; }
        /// <summary>
        /// Частота виміряна
        /// </summary>
        [DataMember]
        public double? RadioControlMeasFreq_MHz { get; set; }
        /// <summary>
        /// Ширина смуги частоти виміряна, кГц
        /// </summary>
        [DataMember]
        public double? RadioControlBandWidth_KHz { get; set; }
        /// <summary>
        /// Відхилення частоти
        /// </summary>
        [DataMember]
        public double? RadioControlDeviationFreq_MHz { get; set; }
        /// <summary>
        /// рівень сигналу (дБм) 
        /// </summary>
        [DataMember]
        public double? Level_dBm { get; set; }
        /// <summary>
        /// Напруженість поля дБМкВ/м (пусте)
        /// </summary>
        [DataMember]
        public double? FieldStrength { get; set; }
        /// <summary>
        /// Дата вимірювання (тільки дата)
        /// </summary>
        [DataMember]
        public DateTime? DateMeas_OnlyDate { get; set; }
        /// <summary>
        /// Дата вимірювання (тільки час)
        /// </summary>
        [DataMember]
        public TimeSpan? DateMeas_OnlyTime { get; set; }
        /// <summary>
        /// Тривалість вимірювання
        /// </summary>
        [DataMember]
        public TimeSpan? DurationMeasurement { get; set; }
        /// <summary>
        /// Координати вимірювання
        /// </summary>
        [DataMember]
        public double? SensorLongitude { get; set; }
        /// <summary>
        /// Координати вимірювання
        /// </summary>
        [DataMember]
        public double? SensorLatitude { get; set; }
        /// <summary>
        /// Координати заявлені
        /// </summary>
        [DataMember]
        public double? Longitude { get; set; }
        /// <summary>
        /// Координати заявлені
        /// </summary>
        [DataMember]
        public double? Latitude { get; set; }
        /// <summary>
        /// Власник РЕЗ
        /// </summary>
        [DataMember]
        public string OwnerName { get; set; }
        /// <summary>
        /// Стандарт
        /// </summary>
        [DataMember]
        public string Standard { get; set; }
        /// <summary>
        /// Адреса
        /// </summary>
        [DataMember]
        public string Address { get; set; }
        /// Номер дозволу
        /// </summary>
        [DataMember]
        public string PermissionNumber { get; set; }
        /// <summary>
        /// Термін дії дозволу (Дата початку)
        /// </summary>
        [DataMember]
        public DateTime? PermissionStart { get; set; }
        /// <summary>
        /// Термін дії дозволу (Дата закінчення)
        /// </summary>
        [DataMember]
        public DateTime? PermissionStop { get; set; }
        /// <summary>
        /// Назва сенсору
        /// </summary>
        [DataMember]
        public string TitleSensor { get; set; }
        /// <summary>
        /// Статус обліку РЕЗ
        /// </summary>
        [DataMember]
        public string CurentStatusStation { get; set; }
        /// <summary>
        /// Результат радіоконтролю для станции
        /// </summary>
        [DataMember]
        public string StatusMeasStation { get; set; }
        /// <summary>
        /// Protocols with emittings
        /// </summary>
        [DataMember]
        public ProtocolsWithEmittings ProtocolsLinkedWithEmittings { get; set; }

    }
}
