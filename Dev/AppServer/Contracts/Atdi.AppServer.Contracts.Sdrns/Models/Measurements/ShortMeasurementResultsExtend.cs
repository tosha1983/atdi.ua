using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Represents main measurements results 
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class ShortMeasurementResultsExtend : ShortMeasurementResults
    {
        /// <summary>
        /// Identifier of measurements results
        /// </summary>
        [DataMember]
        public string SensorName;

    }
}
