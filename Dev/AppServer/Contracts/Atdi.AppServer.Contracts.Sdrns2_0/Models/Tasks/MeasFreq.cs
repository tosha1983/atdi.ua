using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Represents frequency for measurement 
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class MeasFreq
    {
        /// <summary>
        /// frequency, MHz
        /// </summary>
        [DataMember]
        public double Freq;
    }
}
