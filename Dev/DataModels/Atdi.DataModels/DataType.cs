using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels
{
    /// <summary>
    /// The common data types that are used by application services
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public enum DataType
    {
        /// <summary>
        /// The type is string
        /// </summary>
        [EnumMember]
        String,

        /// <summary>
        ///  The type is boolean
        /// </summary>
        [EnumMember]
        Boolean,

        /// <summary>
        /// The type is integer
        /// </summary>
        [EnumMember]
        Integer,

        /// <summary>
        /// The type is DateTime
        /// </summary>
        [EnumMember]
        DateTime,

        /// <summary>
        /// The type is double
        /// </summary>
        [EnumMember]
        Double,

        /// <summary>
        /// The type is float
        /// </summary>
        [EnumMember]
        Float,

        /// <summary>
        /// The type is decimal
        /// </summary>
        [EnumMember]
        Decimal,

        /// <summary>
        /// The type is byte
        /// </summary>
        [EnumMember]
        Byte,

        /// <summary>
        /// The type is byte array
        /// </summary>
        [EnumMember]
        Bytes
    }
}
