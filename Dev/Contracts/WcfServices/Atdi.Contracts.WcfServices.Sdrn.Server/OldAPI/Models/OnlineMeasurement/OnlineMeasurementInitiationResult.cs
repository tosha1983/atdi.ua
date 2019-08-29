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
    /// Модель результата иницирования онлайн измерения
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class OnlineMeasurementInitiationResult
    {
        /// <summary>
        /// Признак того что онлайн измерение разршено сервером и необходимо дождаться готовности устройства
        /// </summary>
        [DataMember]
        public bool Allowed { get; set; }

        /// <summary>
        /// Серверный токен иницирования онлайн измерения. Создаеться сервером в момент иницирования процесса.
        /// </summary>
        [DataMember]
        public byte[] ServerToken { get; set; }

        /// <summary>
        /// Информационное сообщение формируемое сервером.
        /// В случаи отказа сервером иницирования организации процесса онлайн измерения будет передано описание причины
        /// </summary>
        [DataMember]
        public string Message { get; set; }
    }
}
