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
    /// Represents frequency for measurement 
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class StationLevelsByTask
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? Lon;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? Lat;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? Level_dBmkVm;

    }
}

