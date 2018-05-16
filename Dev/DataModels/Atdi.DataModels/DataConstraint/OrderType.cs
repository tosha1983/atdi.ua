using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.DataConstraint
{
    // <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public enum OrderType
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        None = 0,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Ascending = 1,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Descending = 2
    }
}
