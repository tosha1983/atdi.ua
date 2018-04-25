using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.WebQuery
{
    /// <summary>
    /// Represents the node of the tree of the queries
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class QueryTreeNode
    {
        [DataMember]
        public QueryReference QueryRef;
        [DataMember]
        public string Name;
        [DataMember]
        public string Title;
        [DataMember]
        public string Description;
        [DataMember]
        public QueryTreeNode[] ChildNodes;
    }
}
