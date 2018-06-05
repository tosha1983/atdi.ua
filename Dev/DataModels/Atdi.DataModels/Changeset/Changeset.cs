using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels
{
    /// <summary>
    /// Represents the changeset of the web query
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class Changeset
    {
        /// <summary>
        /// The ID of the changeset
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// The action set is included changeset
        /// </summary>
        [DataMember]
        public Action[] Actions { get; set; }
    }
}
