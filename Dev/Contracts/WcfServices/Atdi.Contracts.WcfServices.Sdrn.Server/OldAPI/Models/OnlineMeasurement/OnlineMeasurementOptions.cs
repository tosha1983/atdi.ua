using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    [DataContract(Namespace = Specification.Namespace)]
    public class OnlineMeasurementOptions
    {
        /// <summary>
        /// Идентификатор сенсора, с которым клиент хочет организовать онлайн измерение.
        /// </summary>
        [DataMember]
        public long SensorId { get; set; }

        /// <summary>
        /// Период измерения. Считается с момента получения сервером от устроства статуса готовности.
        /// Используется севером что бы блокировать на этот интрвал времени доступнеость устройства.
        /// А также принять решение о том идет ли проццесс еще имзмерения в случаи анализа доступных сенсоров
        /// без уведомлений со стороны устройств. 
        /// </summary>
        [DataMember]
        public TimeSpan Period { get; set; }
        
    }
}
