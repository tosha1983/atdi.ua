using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    /// <summary>
    /// Represents the tree of the queries
    /// </summary>
    [DataContract]
    public class QueryGroups
    {
        /// <summary>
        /// The groups of the queries
        /// </summary>
        [DataMember]
        public QueryGroup[] Groups { get; set; }

    }
}
