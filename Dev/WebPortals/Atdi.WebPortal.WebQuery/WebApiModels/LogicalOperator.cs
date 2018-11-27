using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public enum LogicalOperator
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        And,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Or
    }
}
