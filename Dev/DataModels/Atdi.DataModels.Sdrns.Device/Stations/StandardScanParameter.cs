using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Parameters of measurement equipment settings for measuring the stations of different standards 
    /// Нормальна ситуация когда один и тотже стандарт требует несколько записей, например для случая когда мы имеем LTE с различными полосами частот.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class StandardScanParameter
    {
        /// <summary>
        /// Standard
        /// </summary>
        [DataMember]
        public string Standard;
        /// <summary>
        /// Maximal relative frequency offset, 10^-6
        /// </summary>
        [DataMember]
        public double? MaxFrequencyRelativeOffset_mk;
        /// <summary>
        /// X level for bandwidth determination accoring to ITU 433 (Appendix 2), дБ
        /// </summary>
        [DataMember]
        public double? XdBLevel_dB;
        /// <summary>
        /// Minimal signal level to start its analyzing, dBm
        /// </summary>
        [DataMember]
        public double? DetectionLevel_dBm;
        /// <summary>
        /// Maximal allowed bandwidth, kHz
        /// </summary>
        [DataMember]
        public double? MaxPermissionBW_kHz;
        /// <summary>
        /// Measurement device parameters
        /// </summary>
        [DataMember]
        public DeviceMeasParam DeviceParam;


    }
}
