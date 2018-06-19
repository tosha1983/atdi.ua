using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.DataConstraint
{
    [KnownType(typeof(StringColumnValue))]
    [KnownType(typeof(BooleanColumnValue))]
    [KnownType(typeof(IntegerColumnValue))]
    [KnownType(typeof(DateTimeColumnValue))]
    [KnownType(typeof(FloatColumnValue))]
    [KnownType(typeof(DoubleColumnValue))]
    [KnownType(typeof(DecimalColumnValue))]
    [KnownType(typeof(ByteColumnValue))]
    [KnownType(typeof(BytesColumnValue))]
    [KnownType(typeof(GuidColumnValue))]
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ColumnValue 
    {
        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DataType DataType { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class StringColumnValue : ColumnValue
    {
        public StringColumnValue()
        {
            this.DataType = DataType.String;
        }

        [DataMember]
        public string Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class BooleanColumnValue : ColumnValue
    {
        public BooleanColumnValue()
        {
            this.DataType = DataType.Boolean;
        }

        [DataMember]
        public bool? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DateTimeColumnValue : ColumnValue
    {
        public DateTimeColumnValue()
        {
            this.DataType = DataType.DateTime;
        }

        [DataMember]
        public DateTime? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class IntegerColumnValue : ColumnValue
    {
        public IntegerColumnValue()
        {
            this.DataType = DataType.Integer;
        }

        [DataMember]
        public int? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DoubleColumnValue : ColumnValue
    {
        public DoubleColumnValue()
        {
            this.DataType = DataType.Double;
        }

        [DataMember]
        public double? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class FloatColumnValue : ColumnValue
    {
        public FloatColumnValue()
        {
            this.DataType = DataType.Float;
        }

        [DataMember]
        public float? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DecimalColumnValue : ColumnValue
    {
        public DecimalColumnValue()
        {
            this.DataType = DataType.Decimal;
        }

        [DataMember]
        public decimal? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ByteColumnValue : ColumnValue
    {
        public ByteColumnValue()
        {
            this.DataType = DataType.Byte;
        }

        [DataMember]
        public byte? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class BytesColumnValue : ColumnValue
    {
        public BytesColumnValue()
        {
            this.DataType = DataType.Bytes;
        }

        [DataMember]
        public byte[] Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class GuidColumnValue : ColumnValue
    {
        public GuidColumnValue()
        {
            this.DataType = DataType.Guid;
        }

        [DataMember]
        public Guid? Value { get; set; }
    }
}
