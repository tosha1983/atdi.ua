using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.DataModels.Sdrns.Server
{
    [Serializable]
    public class InitOnlineMeasurementPipebox
    {
        /// <summary>
        /// Идентификатор сенсора, с которым клиент хочет организовать онлайн измерение.
        /// </summary>
        public long SensorId { get; set; }

        /// <summary>
        /// Sensor name
        /// </summary>
        public string SensorName { get; set; }

        /// <summary>
        /// Sensor tech Id
        /// </summary>
        public string SensorTechId { get; set; }


        /// <summary>
        /// Период измерения. Считается с момента получения сервером от устроства статуса готовности.
        /// Используется севером что бы блокировать на этот интрвал времени доступнеость устройства.
        /// А также принять решение о том идет ли проццесс еще имзмерения в случаи анализа доступных сенсоров
        /// без уведомлений со стороны устройств. 
        /// </summary>
        public TimeSpan Period { get; set; }

        /// <summary>
        /// Идентификатор записи ONLINE_MEAS на стороне MasterServer
        /// </summary>
        public long OnlineMeasMasterId;

        /// <summary>
        /// Локальный идентификатор записи ONLINE_MEAS на стороне AggregationServer или SDRN без ролей
        /// </summary>
        public long OnlineMeasLocalId;
        /// <summary>
        /// 
        /// Признак того что онлайн измерение разршено сервером и необходимо дождаться готовности устройства
        /// </summary>
        public bool Allowed;

        /// <summary>
        /// Серверный токен иницирования онлайн измерения. Создаеться сервером в момент иницирования процесса.
        /// </summary>
        public byte[] ServerToken;

        /// <summary>
        /// Информационное сообщение формируемое сервером.
        /// В случаи отказа сервером иницирования организации процесса онлайн измерения будет передано описание причины
        /// </summary>
        public string Message;
    }

}
