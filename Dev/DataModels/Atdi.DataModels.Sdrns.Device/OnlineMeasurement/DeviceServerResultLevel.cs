using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device.OnlineMeasurement
{
    [Serializable]
    public class DeviceServerResultLevel : DeviceServerData
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
        /// Level
        /// </summary>
        public float[] Level { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double[] Freq_Hz { get; set; }


        /// <summary>
        /// Возвращает в любом случаее текущий установленный
        /// </summary>
        public int RefLevel_dBm { get; set; }

        /// <summary>
        /// Возвращает в любом случаее текущий установленный
        /// </summary>
        public int Att_dB { get; set; }

        /// <summary>
        /// Возвращает в любом случаее текущий установленный
        /// </summary>
        public int PreAmp_dB { get; set; }

        /// <summary>
        /// Возвращает в любом случаее текущий установленный
        /// </summary>
        public double RBW_kHz { get; set; }

        /// <summary>
        /// индикатор overload 
        /// </summary>
        public bool Overload { get; set; }
    }

}
