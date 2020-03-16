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
    public class DataSynchronizationBase
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public long? Id { get; set; }
        /// <summary>
        /// CreatedBy
        /// </summary>
        [DataMember]
        public string CreatedBy { get; set; }

        /// <summary>
        /// DateCreated
        /// </summary>
        [DataMember]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Date start
        /// </summary>
        [DataMember]
        public DateTime DateStart { get; set; }

        /// <summary>
        /// Date end
        /// </summary>
        [DataMember]
        public DateTime DateEnd { get; set; }
    }
}
