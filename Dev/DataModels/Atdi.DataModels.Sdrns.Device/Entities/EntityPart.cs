using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    [DataContract(Namespace = Specification.Namespace)]
    public class EntityPart
    {
        /// <summary>
        /// Entity identifier
        /// </summary>
        [DataMember]
        public string EntityId { get; set; }

        /// <summary>
        /// Entity part index
        /// </summary>
        [DataMember]
        public int PartIndex { get; set; }

        /// <summary>
        /// Defines the Entity part EOF
        /// </summary>
        [DataMember]
        public bool EOF { get; set; }

        /// <summary>
        /// Entity part content
        /// </summary>
        [DataMember]
        public byte[] Content { get; set; }
    }
}
