using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.WebQuery
{
    /// <summary>
    /// Represents the node of the tree of the queries
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class QueriesTreeNode
    {
        [DataMember]
        public QueryToken QueryToken { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// The child nodes
        /// </summary>
        [DataMember]
        public QueriesTreeNode[] Nodes { get; set; }
    }
}
