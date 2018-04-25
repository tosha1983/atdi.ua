using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.WebQuery
{
    /// <summary>
    /// Represents the changeset of the web query
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class QueryChangeset
    {
        [DataMember]
        public QueryReference QueryRef;
        [DataMember]
        public QueryChangesetAction[] Actions;
    }
}
