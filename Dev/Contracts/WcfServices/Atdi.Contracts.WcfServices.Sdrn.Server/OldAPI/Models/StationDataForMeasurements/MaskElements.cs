using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class MaskElements
    {
        [DataMember]
        public double? level; //dB, например -3, - 30, -60
        [DataMember]
        public double? BW; //кГц, ширина спектра при данном уровне
    }
}
