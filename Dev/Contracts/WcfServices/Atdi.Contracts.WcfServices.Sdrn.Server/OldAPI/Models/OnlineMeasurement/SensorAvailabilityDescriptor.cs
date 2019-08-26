using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    /// <summary>
    /// Directional of antenna
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public enum OnlineMeasurementStatus
    {
        /// <summary>
        /// Состояни иницирования онлайн измерения клиентом
        /// Сервре еще не отправил запрос 
        /// </summary>
        [EnumMember]
        Initiation = 0,

        /// <summary>
        /// Состояние при котором сервер отпраивл запрос устройству 
        /// но еще не получил от него подтверждение
        /// </summary>
        [EnumMember]
        WaiteSensor = 1,

        /// <summary>
        /// Состояние при котором сервер отказал в онлайн измерении 
        /// Причина будет раскрыта в сообщении
        /// </summary>
        [EnumMember]
        DeniedByServer = 2,

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

        /// <summary>
        /// Состояние при котором cthdth иницировал отмену/завершения онлайн измерения
        /// Причина будет раскрыта в сообщении
        /// </summary>
        [EnumMember]
        CanceledByServer = 7
    }

    [DataContract(Namespace = Specification.Namespace)]
    public class SensorAvailabilityDescriptor
    {
        /// <summary>
        /// Текущий статус инцированного клиентом онлайн измерения
        /// </summary>
        [DataMember]
        OnlineMeasurementStatus Status { get; set; }

        /// <summary>
        /// Токен выданный устройством для данного клиента.
        /// Нужно использовать при инциирование непосредственного онлайн измерения 
        /// используя открыйтый вебсокет сервером устройств
        /// </summary>
        [DataMember]
        public byte[] SensorToken { get; set; }

        /// <summary>
        /// URL вебсокета на котором устройство ожидает запрос от клиента 
        /// с целью начать онлайн измерение
        /// </summary>
        [DataMember]
        public string WebsocketUrl { get; set; }

        /// <summary>
        /// Информационное сообщение формируемое сервером.
        /// В случаи отказа сервером иницирования организации процесса онлайн измерения будет передано описание причины
        /// </summary>
        [DataMember]
        public string Message { get; set; }

    }
}
