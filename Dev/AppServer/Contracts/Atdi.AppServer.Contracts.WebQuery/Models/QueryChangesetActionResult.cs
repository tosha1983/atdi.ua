using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.WebQuery
{
    /// <summary>
    /// Represents the action with contains into the changeset of the web query
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class QueryChangesetActionResult
    {
        [DataMember]
        public ChangesetActionType Type;
        [DataMember]
        public RecordReference RecordRef;
        [DataMember]
        public bool Success;
        [DataMember]
        public string Message;
    }
}
