using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = CommonServicesSpecification.Namespace)]
    public enum DataConstraintOperation
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Eq,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Geq,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Gt,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Leq,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Lt,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Neq,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        IsNull,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        IsNotNull,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Like,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        NotLike,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        In
    }
}
