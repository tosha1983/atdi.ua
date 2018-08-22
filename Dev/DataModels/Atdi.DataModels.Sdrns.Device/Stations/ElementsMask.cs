using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Элемент маски спектра сигнала 
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class ElementsMask
    {
        /// <summary>
        /// dB, например -3, - 30, -60
        /// </summary>
        [DataMember]
        public double? Level_dB { get; set; }

        /// <summary>
        /// кГц, ширина спектра при данном уровне
        /// </summary>
        [DataMember]
        public double? BW_kHz { get; set; }
}
}
