using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Station sector frequency parameters
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class SectorFrequency
    {
        /// <summary>
        /// Frequency identifier
        /// </summary>
        [DataMember]
        public int? Id; // 

        /// <summary>
        /// Frequency plan identifier
        /// </summary>
        [DataMember]
        public int? PlanId; // 

        /// <summary>
        /// Channel number
        /// </summary>
        [DataMember]
        public int? ChannelNumber;

        /// <summary>
        /// Frequency, MHz
        /// </summary>
        [DataMember]
        public decimal? Frequency_MHz;
    }
}
