using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.DataConstraint
{
    /// <summary>
    /// Specifies a page number and a count of records per page to fetch
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class PagingInfo
    {
        /// <summary>
        /// Gets or sets the number of records returned per page.
        /// </summary>
        [DataMember]
        public int Count { get; set; }

        /// <summary>
        /// The number of page returned.
        /// </summary>
        [DataMember]
        public int Number { get; set; }

        /// <summary>
        /// The token of page returned.
        /// </summary>
        [DataMember]
        public byte[] Token { get; set; }
    }
}
