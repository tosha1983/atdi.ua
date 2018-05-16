using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.DataConstraint
{
    /// <summary>
    /// Represents the exp of the executing query
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class OrderExpression
    {
        /// <summary>
        /// 
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OrderType OrderType { get; set; }
    }
}
