using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.WebQuery
{
    /// <summary>
    /// Represents the metadata to the web query
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class QueryMetadata
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
        public string Techno;
        [DataMember]
        public ColumnMetadata[] Columns;
        [DataMember]
        public QueryParameter[] Parameters;
        [DataMember]
        public string JsonOptions;
        [DataMember]
        public QueryTableStyle TableStyle;

    }
}
