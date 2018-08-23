using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// System information block
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class StationSysInfoBlock
    {
        /// <summary>
        /// Type of system information block
        /// </summary>
        [DataMember]
        public string Type { get; set; }
        /// <summary>
        /// Data of system information block
        /// </summary>
        [DataMember]
        public string Data { get; set; }
    }
}
