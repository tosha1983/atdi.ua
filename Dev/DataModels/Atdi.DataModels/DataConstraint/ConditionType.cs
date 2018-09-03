using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.DataConstraint
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public enum ConditionType
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Complex,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Expression
    }
}
