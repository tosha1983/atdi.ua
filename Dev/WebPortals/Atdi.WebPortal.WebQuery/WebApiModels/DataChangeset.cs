using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    /// <summary>
    /// Represents the changeset of the web query
    /// </summary>
    [DataContract]
    public class DataChangeset
    {
        /// <summary>
        /// The ID of the changeset
        /// </summary>
        [DataMember]
        public Guid ChangesetId { get; set; }

        /// <summary>
        /// The action set is included changeset
        /// </summary>
        [DataMember]
        public DataChangeAction[] Actions { get; set; }

    }
}
