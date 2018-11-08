using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    /// <summary>
    /// Describes the type of comparison for two values (or expressions) in a condition expression
    /// </summary>
    [DataContract]
    public enum ConditionOperator
    {
        /// <summary>
        /// The values are compared for equality.
        /// </summary>
        [EnumMember]
        Equal,
        /// <summary>
        /// The value is greater than or equal to the compared value.
        /// </summary>
        [EnumMember]
        GreaterEqual,
        /// <summary>
        /// The value is greater than the compared value.
        /// </summary>
        [EnumMember]
        GreaterThan,
        /// <summary>
        /// The value is less than or equal to the compared value.
        /// </summary>
        [EnumMember]
        LessEqual,
        /// <summary>
        /// The value is less than the compared value.
        /// </summary>
        [EnumMember]
        LessThan,
        /// <summary>
        /// The two values are not equal.
        /// </summary>
        [EnumMember]
        NotEqual,
        /// <summary>
        /// The value is null.
        /// </summary>
        [EnumMember]
        IsNull,
        /// <summary>
        /// The value is not null. 
        /// </summary>
        [EnumMember]
        IsNotNull,
        /// <summary>
        /// The character string is matched to the specified pattern.
        /// </summary>
        [EnumMember]
        Like,
        /// <summary>
        /// The character string does not match the specified pattern.
        /// </summary>
        [EnumMember]
        NotLike,
        /// <summary>
        /// The value exists in a list of values.
        /// </summary>
        [EnumMember]
        In,
        /// <summary>
        /// The value does not exist in a list of values.
        /// </summary>
        [EnumMember]
        NotIn,
        /// <summary>
        /// The value is between two values.
        /// </summary>
        [EnumMember]
        Between,
        /// <summary>
        /// The value is not between two values.
        /// </summary>
        [EnumMember]
        NotBetween,

        /// <summary>
        /// The character string is matched to the specified pattern.
        /// </summary>
        [EnumMember]
        BeginWith,
        /// <summary>
        /// The character string is matched to the specified pattern.
        /// </summary>
        [EnumMember]
        EndWith,
        /// <summary>
        /// The character string is matched to the specified pattern.
        /// </summary>
        [EnumMember]
        Contains,

        /// <summary>
        /// The character string does not match the specified pattern.
        /// </summary>
        [EnumMember]
        NotBeginWith,
        /// <summary>
        /// The character string does not match the specified pattern.
        /// </summary>
        [EnumMember]
        NotEndWith,
        /// <summary>
        /// The character string does not match the specified pattern.
        /// </summary>
        [EnumMember]
        NotContains

    }
}
