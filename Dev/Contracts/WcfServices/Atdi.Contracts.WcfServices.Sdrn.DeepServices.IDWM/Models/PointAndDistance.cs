using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace Atdi.Contracts.WcfServices.Sdrn.DeepServices.IDWM
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class PointAndDistance
    {
        [DataMember]
        public Point Point { get; set; }

        [DataMember]
        public float Distance { get; set; }
    }
}
