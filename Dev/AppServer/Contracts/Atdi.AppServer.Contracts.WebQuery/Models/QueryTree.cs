using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.WebQuery
{
    /// <summary>
    /// Represents the tree of the queries
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class QueryTree
    {
        [DataMember]
        public QueryTreeNode Root;
        [DataMember]
        public QueryTreeStyle Style;
    }
}
