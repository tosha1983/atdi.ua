using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server.IeStation
{
    /// <summary>
    ///  Data synchronization process
    /// </summary>
    [DataContract(Namespace =Specification.Namespace)]
    public class DataSynchronizationProcess : DataSynchronizationBase
    {
        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        public Status Status { get; set; }

    }
}
