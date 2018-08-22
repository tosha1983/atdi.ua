using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// spectrum occupation
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class SpectrumOccupationMeasParam
    {
        /// <summary>
        /// dBm
        /// </summary>
        [DataMember]
        public double LevelMinOccup { get; set; }

        /// <summary>
        /// FBO - freq bandwidth occupation , FCO - freq channel occupation.
        /// </summary>
        [DataMember]
        public SpectrumOccupationType Type { get; set; }

        /// <summary>
        /// показывает сколько измерений необходимо сделать в канале Т.е. если канал 10 MHz а n_in_chenal = 5 то будем делать измерения с шагом = 10/5 = 2 MHz
        /// </summary>
        [DataMember]
        public int MeasurmentNumber { get; set; }
    }
}
