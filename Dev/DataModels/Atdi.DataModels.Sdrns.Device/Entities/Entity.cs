using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    [DataContract(Namespace = Specification.Namespace)]
    public class Entity 
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string EntityId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ParentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ParentType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ContentType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Encoding { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string HashAlgorithm { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string HashCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int PartIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool EOF { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public byte[] Content { get; set; }
    }

    

}
