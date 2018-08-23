using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns
{
    /// <summary>
    /// Type of spectrum occupation 
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public enum SpectrumOccupationType
    {
        /// <summary>
        /// Frequency band occupancy
        /// </summary>
        [EnumMember]
        FreqBandOccupancy,
        /// <summary>
        /// Frequency channel occupancy
        /// </summary>
        [EnumMember]
        FreqChannelOccupancy
    }
}
