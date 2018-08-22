using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Параметры настроек измерительной аппаратуры для обмера станций определенного стандарта
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class StandarScantParameter
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Standard;
        /// <summary>
        /// Максимальная Относительная растройка частоты, 10^-6
        /// </summary>
        [DataMember]
        public double? MaxFrequencyRelativeOffset_mk;
        /// <summary>
        /// Уровень x для определения BW согласно рекомендации ITU 433 приложение 2, дБ
        /// </summary>
        [DataMember]
        public double? XdBLevel_dB;
        /// <summary>
        /// Минимальный уровень сигнала для начала его анализа, дБм
        /// </summary>
        [DataMember]
        public double? DetectionLevel_dBm;
        /// <summary>
        /// Минимальный уровень сигнала для начала его анализа, дБм
        /// </summary>
        [DataMember]
        public double? MaxPermissionBW_kHz;
        /// <summary>
        /// Параметры приемника
        /// </summary>
        [DataMember]
        public DeviceMeasParam DeviceParam;


    }
}
