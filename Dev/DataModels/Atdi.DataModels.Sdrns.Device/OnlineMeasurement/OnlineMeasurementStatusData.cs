using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Type of measurement
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public enum SensorOnlineMeasurementStatus
    {
        /// <summary>
        /// Состояние при котором сервер отпраивл хапрос и получил  от сенсора/устройства отказ в онлайн измерении 
        /// Причина будет раскрыта в сообщении
        /// </summary>
        [EnumMember]
        DeniedBySensor = 3,

        /// <summary>
        /// Состояние при котором сервер отправил устройству запрос на онлайн измерение
        /// и получил подтверждение с неогбходимо йинформацией дл яфизического поджключени яклиента 
        /// неполсредственно квебсокету устройства.
        /// </summary>
        [EnumMember]
        SonsorReady = 4,

        /// <summary>
        /// Состояние при котором сенсор/устройство иницировало отмену/завершения онлайн измерения
        /// Причина будет раскрыта в сообщении
        /// </summary>
        [EnumMember]
        CanceledBySensor = 5,

        /// <summary>
        /// Состояние при котором клиент иницировал отмену/завершения онлайн измерения
        /// Причина будет раскрыта в сообщении
        /// </summary>
        [EnumMember]
        CanceledByClient = 6,
    }

    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class OnlineMeasurementStatusData
    {
        /// <summary>
        /// The measurement record ID
        /// </summary>
        [DataMember]
        public long OnlineMeasId { get; set; }

        /// <summary>
        /// Новый статус
        /// </summary>
        [DataMember]
        public SensorOnlineMeasurementStatus Status { get; set; }

        [DataMember]
        public string Note { get; set; }
    }
}
