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
        /// The type is undefined
        /// </summary>
        [EnumMember]
        Undefined,

        /// <summary>
        /// The type is System.String.
        /// </summary>
        [EnumMember]
        String,

        /// <summary>
        ///  The type is System.Boolean
        /// </summary>
        [EnumMember]
        Boolean,

        /// <summary>
        /// The type is System.Int32. Signed 32-bit integer. Range -2,147,483,648 to 2,147,483,647
        /// </summary>
        [EnumMember]
        Integer,

        /// <summary>
        /// The type is System.DateTime
        /// </summary>
        [EnumMember]
        DateTime,

        /// <summary>
        /// The type is System.Double. Approximate range ±5.0 × 10^−324 to ±1.7 × 10^308. Precision 15-16 digits
        /// </summary>
        [EnumMember]
        Double,

        /// <summary>
        /// The type is System.Single. Approximate range  ±1.5 x 10^−45 to ±3.4 x 10^38. Precision 7 digits
        /// </summary>
        [EnumMember]
        Float,

        /// <summary>
        /// The type is System.Decimal. Approximate range  ±±1.0 x 10^-28 to ±7.9228 x 10^28. Precision 28-29 digits
        /// </summary>
        [EnumMember]
        Decimal,

        /// <summary>
        /// The type is byte. Range 0 to 255. Unsigned 8-bit integer
        /// </summary>
        [EnumMember]
        Byte,

        /// <summary>
        /// The type is array of System.Byte
        /// </summary>
        [EnumMember]
        Bytes,

        /// <summary>
        /// The type is System.Guid
        /// </summary>
        [EnumMember]
        Guid,

        /// <summary>
        /// The type is System.DateTimeOffset. Represents a point in time, typically expressed as a date and time of day, relative to Coordinated Universal Time (UTC).
        /// </summary>
        [EnumMember]
        DateTimeOffset,

        /// <summary>
        /// The type is System.TimeSpan. Represents a time interval (duration of time or elapsed time) that is measured as a positive or negative number of days, hours, minutes, seconds, and fractions of a second.
        /// </summary>
        [EnumMember]
        Time,

        /// <summary>
        /// The type is System.DateTime without time.
        /// </summary>
        [EnumMember]
        Date,

        /// <summary>
        /// The type is System.Int64. Signed 64-bit integer. Range -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807
        /// </summary>
        [EnumMember]
        Long,

        /// <summary>
        /// The type is System.Int16. Signed 16-bit integer. Range -32,768 to 32,767.
        /// </summary>
        [EnumMember]
        Short,

        /// <summary>
        /// The type is System.Char. Unicode 16-bit character. Range U+0000 to U+ffff.
        /// </summary>
        [EnumMember]
        Char,

        /// <summary>
        /// The type is System.SByte. Signed 8-bit integer. Range -128 to 127.
        /// </summary>
        [EnumMember]
        SignedByte,

        /// <summary>
        /// The type is System.UInt16. Unsigned 16-bit integer. Range 0 to 65,535
        /// </summary>
        [EnumMember]
        UnsignedShort,

        /// <summary>
        /// The type is System.UInt32. Unsigned 32-bit integer. Range 0 to 4,294,967,295
        /// </summary>
        [EnumMember]
        UnsignedInteger,

        /// <summary>
        /// The type is System.UInt64. Unsigned 64-bit integer. Range 0 to 18,446,744,073,709,551,615
        /// </summary>
        [EnumMember]
        UnsignedLong,

        /// <summary>
        /// The object of CLR Type.
        /// </summary>
        [EnumMember]
        ClrType,

        /// <summary>
        /// The value of CLR Enum.
        /// </summary>
        [EnumMember]
        ClrEnum,

        /// <summary>
        /// The value of XML string.
        /// </summary>
        [EnumMember]
        Xml,

        /// <summary>
        /// The value of JSON string.
        /// </summary>
        [EnumMember]
        Json,

        /// <summary>
        /// The type is System.Char[]. Array of Unicode 16-bit character. Range U+0000 to U+ffff.
        /// </summary>
        [EnumMember]
        Chars
    }
}
