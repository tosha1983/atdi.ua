using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class OnlineMeasurementResponseData
    {
        /// <summary>
        /// The measurement record ID
        /// </summary>
        [DataMember]
        public long OnlineMeasId { get; set; }

        [DataMember]
        public bool Conformed { get; set; }

        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Токен генерируемый устройством
        /// </summary>
        [DataMember]
        public byte[] Token { get; set; }

        /// <summary>
        /// URL открытого сервреом устрйоств вебсокета для организации передачи результатов онлайн измерения на клиента 
        /// </summary>
        [DataMember]
        public string WebSocketUrl { get; set; }

    }
}
