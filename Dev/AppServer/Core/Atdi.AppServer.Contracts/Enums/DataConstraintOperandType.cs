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
    public enum DataConstraintOperandType
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Column,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Value,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Values
    }
}
