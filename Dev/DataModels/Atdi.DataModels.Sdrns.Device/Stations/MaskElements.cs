using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    [DataContract(Namespace = Specification.Namespace)]
    public class MaskElements
    {
        /// <summary>
        /// dB, например -3, - 30, -60
        /// </summary>
        [DataMember]
        public double? Level { get; set; }

        /// <summary>
        /// кГц, ширина спектра при данном уровне
        /// </summary>
        [DataMember]
        public double? BW { get; set; }
}
}
