using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.Sdrns.Device
{
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class LevelsDistribution
    {
        [DataMember]
        public int[] Levels { get; set; }
        [DataMember]
        public int[] Count { get; set; }
    }

}
