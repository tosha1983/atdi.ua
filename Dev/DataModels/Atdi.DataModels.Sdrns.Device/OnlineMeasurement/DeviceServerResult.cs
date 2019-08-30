using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device.OnlineMeasurement
{
    [Serializable]
    public class DeviceServerResult : DeviceServerData
    {
        /// <summary>
        /// Номер партии данных
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Метка времени к которой привязаны результаты
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Мощность в dB
        /// </summary>
        public float[] Levels_dB { get; set; }
    }




}
