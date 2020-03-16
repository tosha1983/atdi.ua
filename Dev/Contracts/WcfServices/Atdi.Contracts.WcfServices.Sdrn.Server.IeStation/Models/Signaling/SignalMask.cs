using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server;
using System.Runtime.Serialization;

namespace Atdi.Contracts.WcfServices.Sdrn.Server.IeStation
{
    [DataContract(Namespace = Specification.Namespace)]
    public class SignalMask
    {
        [DataMember]
        public float[] Loss_dB { get; set; }

        [DataMember]
        public double[] Freq_kHz { get; set; }
    }
}
