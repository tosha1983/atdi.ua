using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    ///
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class MeasSdrSOParam //spectrum occupation
    {
        [DataMember]
        public double LevelMinOccup; //dBm
        [DataMember]
        public SpectrumOccupationType TypeSO; //FBO - freq bandwidth occupation , FCO - freq channel occupation.
        [DataMember]
        public int NChenal; // показывает сколько измерений необходимо сделать в канале Т.е. если канал 10 MHz а n_in_chenal = 5 то будем делать измерения с шагом = 10/5 = 2 MHz
    }
}
