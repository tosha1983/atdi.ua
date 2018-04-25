using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Result of measurement
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    [KnownType(typeof(MeasurementResultIdentifier))]
    public class MeasurementResult
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public MeasurementResultIdentifier Id;
    }
}
