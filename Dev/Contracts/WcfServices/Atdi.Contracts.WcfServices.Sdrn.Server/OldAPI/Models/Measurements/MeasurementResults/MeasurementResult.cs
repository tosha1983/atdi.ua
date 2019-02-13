using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    /// <summary>
    /// Result of measurement
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
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
