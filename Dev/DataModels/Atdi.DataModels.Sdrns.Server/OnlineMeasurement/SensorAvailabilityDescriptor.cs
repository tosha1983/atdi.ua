using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.DataModels.Sdrns.Server
{
    /// <summary>
    /// Directional of antenna
    /// </summary>
    [Serializable]
    public enum OnlineMeasurementStatus
    {
        /// <summary>
        /// Состояни иницирования онлайн измерения клиентом
        /// Сервре еще не отправил запрос 
        /// </summary>

        Initiation = 0,

        /// <summary>
        /// Состояние при котором сервер отпраивл запрос устройству 
        /// но еще не получил от него подтверждение
        /// </summary>

        WaitSensor = 1,

        /// <summary>
        /// Состояние при котором сервер отказал в онлайн измерении 
        /// Причина будет раскрыта в сообщении
        /// </summary>

        DeniedByServer = 2,

        /// <summary>
        /// Состояние при котором сервер отпраивл хапрос и получил  от сенсора/устройства отказ в онлайн измерении 
        /// Причина будет раскрыта в сообщении
        /// </summary>

        DeniedBySensor = 3,

        /// <summary>
        /// Состояние при котором сервер отправил устройству запрос на онлайн измерение
        /// и получил подтверждение с неогбходимо йинформацией дл яфизического поджключени яклиента 
        /// неполсредственно квебсокету устройства.
        /// </summary>

        SonsorReady = 4,

        /// <summary>
        /// Состояние при котором сенсор/устройство иницировало отмену/завершения онлайн измерения
        /// Причина будет раскрыта в сообщении
        /// </summary>

        CanceledBySensor = 5,

        /// <summary>
        /// Состояние при котором клиент иницировал отмену/завершения онлайн измерения
        /// Причина будет раскрыта в сообщении
        /// </summary>

        CanceledByClient = 6,

        /// <summary>
        /// Состояние при котором cthdth иницировал отмену/завершения онлайн измерения
        /// Причина будет раскрыта в сообщении
        /// </summary>

        CanceledByServer = 7
    }
}
