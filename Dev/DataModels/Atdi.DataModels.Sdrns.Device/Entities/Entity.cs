using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class Entity 
    {
        /// <summary>
        /// Entity identifier
        /// </summary>
        [DataMember]
        public string EntityId { get; set; }

        /// <summary>
        /// Entity name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Entity description
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Entity parent identifier
        /// </summary>
        [DataMember]
        public string ParentId { get; set; }

        /// <summary>
        /// Entity parent type
        /// </summary>
        [DataMember]
        public string ParentType { get; set; }

        /// <summary>
        /// Entity content type
        /// </summary>
        [DataMember]
        public string ContentType { get; set; }

        /// <summary>
        /// Entity encoding
        /// </summary>
        [DataMember]
        public string Encoding { get; set; }

        /// <summary>
        /// Entity hash algorithm
        /// </summary>
        [DataMember]
        public string HashAlgorithm { get; set; }

        /// <summary>
        /// Entity hash code
        /// </summary>
        [DataMember]
        public string HashCode { get; set; }

        /// <summary>
        /// Entity part index
        /// </summary>
        [DataMember]
        public int PartIndex { get; set; }

        /// <summary>
        /// Defines the Entity EOF
        /// </summary>
        [DataMember]
        public bool EOF { get; set; }

        /// <summary>
        /// Entity content
        /// </summary>
        [DataMember]
        public byte[] Content { get; set; }
    }

    

}
