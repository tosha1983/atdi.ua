using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    // <summary>
    /// 
    /// </summary>
    [DataContract]
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
