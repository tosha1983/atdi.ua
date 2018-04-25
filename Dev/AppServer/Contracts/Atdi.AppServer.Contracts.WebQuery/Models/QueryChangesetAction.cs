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
    [KnownType(typeof(QueryChangesetCreationAction))]
    [KnownType(typeof(QueryChangesetUpdationAction))]
    [KnownType(typeof(QueryChangesetDeleteionAction))]
    public class QueryChangesetAction
    {
        [DataMember]
        public ChangesetActionType Type;
    }
}
