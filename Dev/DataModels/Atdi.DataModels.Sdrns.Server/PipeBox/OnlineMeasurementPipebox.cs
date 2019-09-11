using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.DataModels.Sdrns.Server
{

    [Serializable]
    public class OnlineMeasurementPipebox
    {
        public long OnlineMeasId;

        /// <summary>
        /// Серверный токен иницирования онлайн измерения. Создаеться сервером в момент иницирования процесса.
        /// </summary>
        public byte[] ServerToken;

        public string SensorName { get; set; }

        public string SensorTechId { get; set; }

        public string AggregationServerInstance { get; set; }

        public TimeSpan Period { get; set; }

    }

}
