using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.WebQuery
{
    /// <summary>
    /// Represents the data of the result of the executed query
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
   
    public class QueryResult
    {
        /// <summary>
        /// The ID of the option set
        /// </summary>
        [DataMember]
        public Guid OptionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public QueryToken Token { get; set; }

        /// <summary>
        /// The feched data
        /// </summary>
        [DataMember]
        public DataSet Dataset { get; set; }

    }
}
