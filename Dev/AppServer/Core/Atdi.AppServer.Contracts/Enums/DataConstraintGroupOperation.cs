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
    public enum DataConstraintGroupOperation
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
