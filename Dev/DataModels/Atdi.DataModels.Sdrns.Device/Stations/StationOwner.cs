using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Station owner information
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class StationOwner
    {
        /// <summary>
        /// Owner ID
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Owner name
        /// </summary>
        [DataMember]
        public string OwnerName { get; set; }

        /// <summary>
        /// OKPO
        /// </summary>
        [DataMember]
        public string OKPO { get; set; }

        /// <summary>
        /// ZIP code
        /// </summary>
        [DataMember]
        public string Zip { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        [DataMember]
        public string Address { get; set; }
    }
}
