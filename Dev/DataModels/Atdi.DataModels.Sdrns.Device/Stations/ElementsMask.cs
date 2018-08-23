using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Elements of signal spectrum mask
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class ElementsMask
    {
        /// <summary>
        /// Level, dB (for example: -3, - 30, -60)
        /// </summary>
        [DataMember]
        public double? Level_dB { get; set; }

        /// <summary>
        /// Spectrum bandwidth on a given level, kHz
        /// </summary>
        [DataMember]
        public double? BW_kHz { get; set; }
}
}
