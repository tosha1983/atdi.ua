using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.DataConstraint
{
    [KnownType(typeof(StringValuesOperand))]
    [KnownType(typeof(BooleanValuesOperand))]
    [KnownType(typeof(IntegerValuesOperand))]
    [KnownType(typeof(DateTimeValuesOperand))]
    [KnownType(typeof(FloatValuesOperand))]
    [KnownType(typeof(DoubleValuesOperand))]
    [KnownType(typeof(DecimalValuesOperand))]
    [KnownType(typeof(ByteValuesOperand))]
    [KnownType(typeof(BytesValuesOperand))]
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ValuesOperand : Operand
    {
        public ValuesOperand()
        {
            this.Type = OperandType.Values;
        }

        [DataMember]
        public DataType DataType { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class StringValuesOperand : ValuesOperand
    {
        public StringValuesOperand()
        {
            this.DataType = DataType.String;
        }

        [DataMember]
        public string[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class BooleanValuesOperand : ValuesOperand
    {
        public BooleanValuesOperand()
        {
            this.DataType = DataType.Boolean;
        }

        [DataMember]
        public bool?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DateTimeValuesOperand : ValuesOperand
    {
        public DateTimeValuesOperand()
        {
            this.DataType = DataType.DateTime;
        }

        [DataMember]
        public DateTime?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class IntegerValuesOperand : ValuesOperand
    {
        public IntegerValuesOperand()
        {
            this.DataType = DataType.Integer;
        }

        [DataMember]
        public int?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DoubleValuesOperand : ValuesOperand
    {
        public DoubleValuesOperand()
        {
            this.DataType = DataType.Double;
        }

        [DataMember]
        public double?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class FloatValuesOperand : ValuesOperand
    {
        public FloatValuesOperand()
        {
            this.DataType = DataType.Float;
        }

        [DataMember]
        public float?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DecimalValuesOperand : ValuesOperand
    {
        public DecimalValuesOperand()
        {
            this.DataType = DataType.Decimal;
        }

        [DataMember]
        public decimal?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ByteValuesOperand : ValuesOperand
    {
        public ByteValuesOperand()
        {
            this.DataType = DataType.Byte;
        }

        [DataMember]
        public byte?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class BytesValuesOperand : ValuesOperand
    {
        public BytesValuesOperand()
        {
            this.DataType = DataType.Bytes;
        }

        [DataMember]
        public byte?[][] Values { get; set; }
    }
}
