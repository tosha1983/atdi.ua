using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Данные овнера станции
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class StationOwner
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string OwnerName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string OKPO { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Zip { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Address { get; set; }
    }
}
