using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.WebQuery
{
    /// <summary>
    /// Represents the metadata to the web query
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class QueryMetadata
    {
        [DataMember]
        public QueryToken Token { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public ColumnMetadata[] Columns { get; set; }

    }
}
