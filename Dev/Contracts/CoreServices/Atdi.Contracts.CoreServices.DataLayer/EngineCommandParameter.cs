using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public class EngineCommandParameter
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public DataType DataType { get; set; }

        public EngineParameterDirection Direction { get; set; }

        public override string ToString()
        {
            var showValue = Value;
            var type = Value?.GetType();
            if (type == typeof(string) )
            {
                if (showValue != null)
                {
                    var str = (string)showValue;
                    if (str.Length > 50)
                    {
                        showValue = str.Substring(0, 50);
                    }
                }
                
            }
            else if(type == typeof(char[]))
            {
                if (showValue != null)
                {
                    var str = (char[])showValue;
                    if (str.Length > 50)
                    {
                        showValue = "Big char array: " + str.Length;
                    }
                }

            }
            return $"Name = '{this.Name}', Direction = '{Direction}', DataType = '{this.DataType}', VarType = '{Value?.GetType().FullName}', Value = '{showValue}'";
        }

        public void SetValue(object value)
        {
            if (value == null)
            {
                this.Value = null;
            }
            else
            {
                var type = value.GetType();
                switch (this.DataType)
                {
                    case DataType.String:
                        if (type == typeof(string))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            this.Value = Convert.ToString(value);
                        }
                        break;
                    case DataType.Boolean:
                        if (type == typeof(bool) || type == typeof(bool?))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            this.Value = Convert.ToBoolean(value);
                        }
                        break;
                    case DataType.Integer:
                        if (type == typeof(int) || type == typeof(int?))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            this.Value = Convert.ToInt32(value);
                        }
                        break;
                    case DataType.DateTime:
                        if (type == typeof(DateTime) || type == typeof(DateTime?))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            this.Value = Convert.ToDateTime(value);
                        }
                        break;
                    case DataType.Double:
                        if (type == typeof(double) || type == typeof(double?))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            this.Value = Convert.ToDouble(value);
                        }
                        break;
                    case DataType.Float:
                        if (type == typeof(float) || type == typeof(float?))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            this.Value = (float)Convert.ToDouble(value);
                        }
                        break;
                    case DataType.Decimal:
                        if (type == typeof(decimal) || type == typeof(decimal?))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            this.Value = Convert.ToDecimal(value);
                        }
                        break;
                    case DataType.Byte:
                        if (type == typeof(byte) || type == typeof(byte?))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            this.Value = Convert.ToByte(value);
                        }
                        break;
                    case DataType.Bytes:
                        if (type == typeof(byte[]))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            this.Value = (byte[])value;
                        }
                        break;
                    case DataType.Guid:
                        if (type == typeof(Guid) || type == typeof(Guid?))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            throw new InvalidCastException($"Unsupported the converting GUID type from type with name {type.FullName}");
                        }
                        break;
                    case DataType.DateTimeOffset:
                        if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?) || type == typeof(DateTime) || type == typeof(DateTime?))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            throw new InvalidCastException($"Unsupported the converting DateTimeOffset type from type with name {type.FullName}");
                        }
                        break;
                    case DataType.Time:
                        if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            throw new InvalidCastException($"Unsupported the converting TimeSpan type from type with name {type.FullName}");
                        }
                        break;
                    case DataType.Date:
                        if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?) || type == typeof(DateTime) || type == typeof(DateTime?))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            throw new InvalidCastException($"Unsupported the converting Date type from type with name {type.FullName}");
                        }
                        break;
                    case DataType.Long:
                        if (type == typeof(long) || type == typeof(long?))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            this.Value = Convert.ToInt64(value);
                        }
                        break;
                    case DataType.Short:
                        if (type == typeof(short) || type == typeof(short?))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            this.Value = Convert.ToInt16(value);
                        }
                        break;
                    case DataType.Char:
                        if (type == typeof(char) || type == typeof(char?))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            this.Value = Convert.ToChar(value);
                        }
                        break;
                    case DataType.SignedByte:
                        if (type == typeof(sbyte) || type == typeof(sbyte?))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            this.Value = Convert.ToSByte(value);
                        }
                        break;
                    case DataType.UnsignedShort:
                        if (type == typeof(ushort) || type == typeof(ushort?))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            this.Value = Convert.ToUInt16(value);
                        }
                        break;
                    case DataType.UnsignedInteger:
                        if (type == typeof(uint) || type == typeof(uint?))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            this.Value = Convert.ToUInt32(value);
                        }
                        break;
                    case DataType.UnsignedLong:
                        if (type == typeof(ulong) || type == typeof(ulong?))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            this.Value = Convert.ToUInt64(value);
                        }
                        break;
                    case DataType.ClrType:
                        this.Value = value;
                        break;
                    case DataType.ClrEnum:
                        this.Value = value;
                        break;
                    case DataType.Xml:
                        this.Value = value;
                        break;
                    case DataType.Json:
                        this.Value = value;
                        break;
                    case DataType.Chars:
                        if (type == typeof(char[]))
                        {
                            this.Value = value;
                        }
                        else
                        {
                            this.Value = (char[])value;
                        }
                        break;
                    case DataType.Undefined:
                    default:
                        throw new InvalidCastException($"Unsupported parameter type of Engine Command {this.DataType}");
                }
            }
        }
    }


}
