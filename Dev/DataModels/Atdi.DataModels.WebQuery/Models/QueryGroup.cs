using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.WebQuery
{
    /// <summary>
    /// Represents the group of the queries
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class QueryGroup
    {
        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int? CanCreate { get; set; }
        [DataMember]
        public int? CanModify { get; set; }
        [DataMember]
        public int? CanDelete { get; set; }
        /// <summary>
        /// The tokens of the available queries
        /// </summary>
        [DataMember]
        public QueryToken[] QueryTokens { get; set; }
    }
}
