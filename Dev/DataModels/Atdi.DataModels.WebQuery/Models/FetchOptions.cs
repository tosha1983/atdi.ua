using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.DataConstraint;

namespace Atdi.DataModels.WebQuery
{
    /// <summary>
    /// Represents the options of the executing query
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class FetchOptions
    {
        /// <summary>
        /// The columns to include the result.
        /// </summary>
        [DataMember]
        public string[] Columns { get; set; }

        /// <summary>
        /// The complex conditions and logical condition expressions that filter the results
        /// </summary>
        [DataMember]
        public Condition Condition { get; set; }

        /// <summary>
        /// The orders info
        /// </summary>
        [DataMember]
        public OrderExpression[] Orders { get; set; }

        /// <summary>
        /// The paging info
        /// </summary>
        [DataMember]
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// The number of records to be selected.
        /// </summary>
        [DataMember]
        public int Limit { get; set; }
    }
}
