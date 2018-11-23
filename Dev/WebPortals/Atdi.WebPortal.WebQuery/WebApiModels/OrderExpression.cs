using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    /// <summary>
    /// Represents the exp of the executing query
    /// </summary>
    [DataContract]
    public class OrderExpression
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ColumnName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public OrderType OrderType { get; set; }
    }
}
