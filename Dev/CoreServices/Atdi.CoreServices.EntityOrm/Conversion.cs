using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels;

namespace Atdi.CoreServices.EntityOrm
{

    public sealed class Convesrion
    {
        private static IFormatProvider CultureEnUs = new System.Globalization.CultureInfo("en-US");


        public static ColumnValue ConversionBit(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value!=null ? value.ToString() : "false", out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value!=null ? value.ToString() : "0", out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value!=null ? (value.ToString() == "1" ? (bool?)true : (bool?)false) : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;

                case "Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value!=null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Time":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Date":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTime":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTimeOffset":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }


        public static ColumnValue ConversionBool(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value != null ? value.ToString() : "false", out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value != null ? value.ToString() : "0", out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value != null ? (value.ToString() == "1" ? (bool?)true : (bool?)false) : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;

                case "Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value!=null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Time":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Date":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTime":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTimeOffset":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }

        public static ColumnValue ConversionByte(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value != null ? value.ToString() : "false", out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value != null ? value.ToString() : "0", out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value != null ? (value.ToString() == "1" ? (bool?)true : (bool?)false) : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;

                case "Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Time":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Date":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTime":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTimeOffset":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }

        public static ColumnValue ConversionBytes(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Char":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "String":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Short":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "UnsignedShort":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Integer":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "UnsignedInteger":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Long":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "UnsignedLong":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Double":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Float":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Decimal":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    result = new GuidColumnValue
                    {
                        Value = (value == null) ? (Guid?)null : (Guid.Parse(value.ToString()) as Guid?),
                        Name = nameColumn
                    };
                    break;
                case "Time":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Date":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTime":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTimeOffset":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }


        public static ColumnValue ConversionBlob(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value != null ? value.ToString() : "false", out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value != null ? value.ToString() : "0", out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value != null ? (value.ToString() == "1" ? (bool?)true : (bool?)false) : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;

                case "Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    result = new GuidColumnValue
                    {
                        Value = (value == null) ? (Guid?)null : (Guid.Parse(value.ToString()) as Guid?),
                        Name = nameColumn
                    };
                    break;

                case "Time":
                    result = new TimeColumnValue
                    {
                        Value = (value == null) ? (TimeSpan?)null : (System.TimeSpan.Parse(value.ToString()) as TimeSpan?),
                        Name = nameColumn
                    };
                    break;

                case "Date":
                    result = new DateColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTime":
                    result = new DateTimeColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTimeOffset":
                    result = new DateTimeOffsetColumnValue
                    {
                        Value = (value == null) ? (DateTimeOffset?)null : (System.DateTimeOffset.Parse(value.ToString()) as DateTimeOffset?),
                        Name = nameColumn
                    };
                    break;

                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }


        public static ColumnValue ConversionInt08(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value != null ? value.ToString() : "false", out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value != null ? value.ToString() : "0", out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value != null ? (value.ToString() == "1" ? (bool?)true : (bool?)false) : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;

                case "Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Time":
                    result = new TimeColumnValue
                    {
                        Value = (value == null) ? (TimeSpan?)null : (System.TimeSpan.Parse(value.ToString()) as TimeSpan?),
                        Name = nameColumn
                    };
                    break;
                case "Date":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTime":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTimeOffset":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }



        public static ColumnValue ConversionInt16(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value != null ? value.ToString() : "false", out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value != null ? value.ToString() : "0", out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value != null ? (value.ToString() == "1" ? (bool?)true : (bool?)false) : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;

                case "Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Time":
                    result = new TimeColumnValue
                    {
                        Value = (value == null) ? (TimeSpan?)null : (System.TimeSpan.Parse(value.ToString()) as TimeSpan?),
                        Name = nameColumn
                    };
                    break;
                case "Date":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTime":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTimeOffset":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }

        public static ColumnValue ConversionInt32(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value != null ? value.ToString() : "false", out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value != null ? value.ToString() : "0", out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value != null ? (value.ToString() == "1" ? (bool?)true : (bool?)false) : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;

                case "Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Time":
                    result = new TimeColumnValue
                    {
                        Value = (value == null) ? (TimeSpan?)null : (System.TimeSpan.Parse(value.ToString()) as TimeSpan?),
                        Name = nameColumn
                    };
                    break;
                case "Date":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTime":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTimeOffset":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }

        public static ColumnValue ConversionInt64(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value != null ? value.ToString() : "false", out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value != null ? value.ToString() : "0", out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value != null ? (value.ToString() == "1" ? (bool?)true : (bool?)false) : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;

                case "Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Time":
                    result = new TimeColumnValue
                    {
                        Value = (value == null) ? (TimeSpan?)null : (System.TimeSpan.Parse(value.ToString()) as TimeSpan?),
                        Name = nameColumn
                    };
                    break;
                case "Date":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTime":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTimeOffset":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }


        public static ColumnValue ConversionMoney(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value != null ? value.ToString() : "false", out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value != null ? value.ToString() : "0", out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value != null ? (value.ToString() == "1" ? (bool?)true : (bool?)false) : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;

                case "Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Bytes":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "SignedByte":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Guid":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Time":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Date":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTime":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTimeOffset":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }


        public static ColumnValue ConversionDecimal(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value != null ? value.ToString() : "false", out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value != null ? value.ToString() : "0", out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value != null ? (value.ToString() == "1" ? (bool?)true : (bool?)false) : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;

                case "Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Bytes":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "SignedByte":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Guid":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Time":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Date":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTime":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTimeOffset":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }

        public static ColumnValue ConversionFloat(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value != null ? value.ToString() : "false", out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value != null ? value.ToString() : "0", out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value != null ? (value.ToString() == "1" ? (bool?)true : (bool?)false) : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;

                case "Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Bytes":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "SignedByte":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Guid":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Time":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Date":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTime":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTimeOffset":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }

        public static ColumnValue ConversionDouble(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value != null ? value.ToString() : "false", out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value != null ? value.ToString() : "0", out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value != null ? (value.ToString() == "1" ? (bool?)true : (bool?)false) : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;

                case "Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Bytes":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "SignedByte":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Guid":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Time":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Date":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTime":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTimeOffset":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }

        public static ColumnValue ConversionTime(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Char":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Time":
                    result = new TimeColumnValue
                    {
                        Value = (value == null) ? (TimeSpan?)null : (System.TimeSpan.Parse(value.ToString()) as TimeSpan?),
                        Name = nameColumn
                    };
                    break;

                case "Date":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTime":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTimeOffset":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }

        public static ColumnValue ConversionDate(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Char":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Time":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

                case "Date":
                    result = new DateColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTime":
                    result = new DateTimeColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTimeOffset":
                    result = new DateTimeOffsetColumnValue
                    {
                        Value = (value == null) ? (DateTimeOffset?)null : (System.DateTimeOffset.Parse(value.ToString()) as DateTimeOffset?),
                        Name = nameColumn
                    };
                    break;

                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }

        public static ColumnValue ConversionDateTime(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Char":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Time":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

                case "Date":
                    result = new DateColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTime":
                    result = new DateTimeColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTimeOffset":
                    result = new DateTimeOffsetColumnValue
                    {
                        Value = (value == null) ? (DateTimeOffset?)null : (System.DateTimeOffset.Parse(value.ToString()) as DateTimeOffset?),
                        Name = nameColumn
                    };
                    break;

                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }

        public static ColumnValue ConversionDateTimeOffset(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Char":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Time":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

                case "Date":
                    result = new DateColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTime":
                    result = new DateTimeColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTimeOffset":
                    result = new DateTimeOffsetColumnValue
                    {
                        Value = (value == null) ? (DateTimeOffset?)null : (System.DateTimeOffset.Parse(value.ToString()) as DateTimeOffset?),
                        Name = nameColumn
                    };
                    break;

                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }

        public static ColumnValue ConversionGuid(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Char":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "UnsignedShort":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

                case "Integer":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "UnsignedInteger":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Long":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "UnsignedLong":
                   throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Float":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Decimal":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Byte":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Guid":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Time":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Date":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTime":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTimeOffset":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }


        public static ColumnValue ConversionJson(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Char":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "String":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Short":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "UnsignedShort":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Integer":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "UnsignedInteger":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Long":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "UnsignedLong":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Float":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Decimal":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Byte":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Bytes":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "SignedByte":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Guid":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Time":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Date":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTime":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTimeOffset":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    break;

                   
            }
            return result;
        }

        public static ColumnValue ConversionXml(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Char":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "String":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Short":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "UnsignedShort":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Integer":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "UnsignedInteger":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Long":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "UnsignedLong":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Float":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Decimal":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Byte":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Bytes":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "SignedByte":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Guid":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Time":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Date":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTime":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "DateTimeOffset":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    break;

            }
            return result;
        }


        public static ColumnValue ConversionNchar(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value != null ? value.ToString() : "false", out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value != null ? value.ToString() : "0", out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value != null ? (value.ToString() == "1" ? (bool?)true : (bool?)false) : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;

                case "Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    result = new GuidColumnValue
                    {
                        Value = (value == null) ? (Guid?)null : (Guid.Parse(value.ToString()) as Guid?),
                        Name = nameColumn
                    };
                    break;

                case "Time":
                    result = new TimeColumnValue
                    {
                        Value = (value == null) ? (TimeSpan?)null : (System.TimeSpan.Parse(value.ToString()) as TimeSpan?),
                        Name = nameColumn
                    };
                    break;

                case "Date":
                    result = new DateColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTime":
                    result = new DateTimeColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTimeOffset":
                    result = new DateTimeOffsetColumnValue
                    {
                        Value = (value == null) ? (DateTimeOffset?)null : (System.DateTimeOffset.Parse(value.ToString()) as DateTimeOffset?),
                        Name = nameColumn
                    };
                    break;

                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }

        public static ColumnValue ConversionNvarChar(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value != null ? value.ToString() : "false", out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value != null ? value.ToString() : "0", out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value != null ? (value.ToString() == "1" ? (bool?)true : (bool?)false) : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;

                case "Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value!=null ? ( value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    result = new GuidColumnValue
                    {
                        Value = (value == null) ? (Guid?)null : (Guid.Parse(value.ToString()) as Guid?),
                        Name = nameColumn
                    };
                    break;

                case "Time":
                    result = new TimeColumnValue
                    {
                        Value = (value == null) ? (TimeSpan?)null : (System.TimeSpan.Parse(value.ToString()) as TimeSpan?),
                        Name = nameColumn
                    };
                    break;

                case "Date":
                    result = new DateColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTime":
                    result = new DateTimeColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTimeOffset":
                    result = new DateTimeOffsetColumnValue
                    {
                        Value = (value == null) ? (DateTimeOffset?)null : (System.DateTimeOffset.Parse(value.ToString()) as DateTimeOffset?),
                        Name = nameColumn
                    };
                    break;

                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }

        public static ColumnValue ConversionNText(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value != null ? value.ToString() : "false", out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value != null ? value.ToString() : "0", out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value != null ? (value.ToString() == "1" ? (bool?)true : (bool?)false) : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;

                case "Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    result = new GuidColumnValue
                    {
                        Value = (value == null) ? (Guid?)null : (Guid.Parse(value.ToString()) as Guid?),
                        Name = nameColumn
                    };
                    break;

                case "Time":
                    result = new TimeColumnValue
                    {
                        Value = (value == null) ? (TimeSpan?)null : (System.TimeSpan.Parse(value.ToString()) as TimeSpan?),
                        Name = nameColumn
                    };
                    break;

                case "Date":
                    result = new DateColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTime":
                    result = new DateTimeColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTimeOffset":
                    result = new DateTimeOffsetColumnValue
                    {
                        Value = (value == null) ? (DateTimeOffset?)null : (System.DateTimeOffset.Parse(value.ToString()) as DateTimeOffset?),
                        Name = nameColumn
                    };
                    break;

                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }

        public static ColumnValue ConversionChar(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value != null ? value.ToString() : "false", out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value != null ? value.ToString() : "0", out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value != null ? (value.ToString() == "1" ? (bool?)true : (bool?)false) : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;

                case "Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    result = new GuidColumnValue
                    {
                        Value = (value == null) ? (Guid?)null : (Guid.Parse(value.ToString()) as Guid?),
                        Name = nameColumn
                    };
                    break;

                case "Time":
                    result = new TimeColumnValue
                    {
                        Value = (value == null) ? (TimeSpan?)null : (System.TimeSpan.Parse(value.ToString()) as TimeSpan?),
                        Name = nameColumn
                    };
                    break;

                case "Date":
                    result = new DateColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTime":
                    result = new DateTimeColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTimeOffset":
                    result = new DateTimeOffsetColumnValue
                    {
                        Value = (value == null) ? (DateTimeOffset?)null : (System.DateTimeOffset.Parse(value.ToString()) as DateTimeOffset?),
                        Name = nameColumn
                    };
                    break;

                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }



        public static ColumnValue ConversionVarChar(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value != null ? value.ToString() : "false", out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value != null ? value.ToString() : "0", out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value != null ? (value.ToString() == "1" ? (bool?)true : (bool?)false) : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;

                case "Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    result = new GuidColumnValue
                    {
                        Value = (value == null) ? (Guid?)null : (Guid.Parse(value.ToString()) as Guid?),
                        Name = nameColumn
                    };
                    break;

                case "Time":
                    result = new TimeColumnValue
                    {
                        Value = (value == null) ? (TimeSpan?)null : (System.TimeSpan.Parse(value.ToString()) as TimeSpan?),
                        Name = nameColumn
                    };
                    break;

                case "Date":
                    result = new DateColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTime":
                    result = new DateTimeColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTimeOffset":
                    result = new DateTimeOffsetColumnValue
                    {
                        Value = (value == null) ? (DateTimeOffset?)null : (System.DateTimeOffset.Parse(value.ToString()) as DateTimeOffset?),
                        Name = nameColumn
                    };
                    break;

                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }

        public static ColumnValue ConversionText(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.CodeVarType.ToString())
            {

                case "Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value != null ? value.ToString() : "false", out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value != null ? value.ToString() : "0", out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value != null ? (value.ToString() == "1" ? (bool?)true : (bool?)false) : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;

                case "Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "String":
                    result = new StringColumnValue
                    {
                        Value = value != null ? (value.ToString() ?? null) : null,
                        Name = nameColumn
                    };
                    break;
                case "Short":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedShort":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;

                case "Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "UnsignedInteger":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "Long":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };

                    break;
                case "UnsignedLong":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "Float":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };

                    break;
                case "Bytes":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "SignedByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;

                case "Guid":
                    result = new GuidColumnValue
                    {
                        Value = (value == null) ? (Guid?)null : (Guid.Parse(value.ToString()) as Guid?),
                        Name = nameColumn
                    };
                    break;

                case "Time":
                    result = new TimeColumnValue
                    {
                        Value = (value == null) ? (TimeSpan?)null : (System.TimeSpan.Parse(value.ToString()) as TimeSpan?),
                        Name = nameColumn
                    };
                    break;

                case "Date":
                    result = new DateColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTime":
                    result = new DateTimeColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;

                case "DateTimeOffset":
                    result = new DateTimeOffsetColumnValue
                    {
                        Value = (value == null) ? (DateTimeOffset?)null : (System.DateTimeOffset.Parse(value.ToString()) as DateTimeOffset?),
                        Name = nameColumn
                    };
                    break;

                case "Json":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "Xml":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrEnum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "ClrType":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");

            }
            return result;
        }
    }
}
