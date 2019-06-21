using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public sealed class EngineCommand
    {
        private readonly IDictionary<string, EngineCommandParameter> _parameters;

        public EngineCommand()
        {
            this._parameters = new Dictionary<string, EngineCommandParameter>();
        }

        public string Text { get; set; }


        public IDictionary<string, EngineCommandParameter> Parameters => _parameters;

        public override string ToString()
        {
            if (this.Parameters.Count == 0)
            {
                return $"Command: {this.Text}";
            }

            return $"Command: {this.Text}" + Environment.NewLine + $"Parameters: count = {this.Parameters.Count}";
        }

        public void AddParameter(string name, DataType dataType, object value, EngineParameterDirection direction = EngineParameterDirection.Input)
        {
            var parameter = new EngineCommandParameter
            {
                Name = name,
                DataType = dataType,
                Direction = direction
            };
            if (value == null)
            {
                parameter.Value = null;
            }
            else
            {
                var type = value.GetType();
                switch (dataType)
                {
                    case DataType.String:
                        if (type == typeof(string))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            parameter.Value = Convert.ToString(value);
                        }
                        break;
                    case DataType.Boolean:
                        if (type == typeof(bool) || type == typeof(bool?))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            parameter.Value = Convert.ToBoolean(value);
                        }
                        break;
                    case DataType.Integer:
                        if (type == typeof(int) || type == typeof(int?))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            parameter.Value = Convert.ToInt32(value);
                        }
                        break;
                    case DataType.DateTime:
                        if (type == typeof(DateTime) || type == typeof(DateTime?))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            parameter.Value = Convert.ToDateTime(value);
                        }
                        break;
                    case DataType.Double:
                        if (type == typeof(double) || type == typeof(double?))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            parameter.Value = Convert.ToDouble(value);
                        }
                        break;
                    case DataType.Float:
                        if (type == typeof(float) || type == typeof(float?))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            parameter.Value = (float)Convert.ToDouble(value);
                        }
                        break;
                    case DataType.Decimal:
                        if (type == typeof(decimal) || type == typeof(decimal?))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            parameter.Value = Convert.ToDecimal(value);
                        }
                        break;
                    case DataType.Byte:
                        if (type == typeof(byte) || type == typeof(byte?))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            parameter.Value = Convert.ToByte(value);
                        }
                        break;
                    case DataType.Bytes:
                        if (type == typeof(byte[]))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            parameter.Value = (byte[])value;
                        }
                        break;
                    case DataType.Guid:
                        if (type == typeof(Guid) || type == typeof(Guid?))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            throw new InvalidCastException($"Unsupported the converting GUID type from type with name {type.FullName}");
                        }
                        break;
                    case DataType.DateTimeOffset:
                        if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?) || type == typeof(DateTime) || type == typeof(DateTime?))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            throw new InvalidCastException($"Unsupported the converting DateTimeOffset type from type with name {type.FullName}");
                        }
                        break;
                    case DataType.Time:
                        if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            throw new InvalidCastException($"Unsupported the converting TimeSpan type from type with name {type.FullName}");
                        }
                        break;
                    case DataType.Date:
                        if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?) || type == typeof(DateTime) || type == typeof(DateTime?))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            throw new InvalidCastException($"Unsupported the converting Date type from type with name {type.FullName}");
                        }
                        break;
                    case DataType.Long:
                        if (type == typeof(long) || type == typeof(long?))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            parameter.Value = Convert.ToInt64(value);
                        }
                        break;
                    case DataType.Short:
                        if (type == typeof(short) || type == typeof(short?))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            parameter.Value = Convert.ToInt16(value);
                        }
                        break;
                    case DataType.Char:
                        if (type == typeof(char) || type == typeof(char?))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            parameter.Value = Convert.ToChar(value);
                        }
                        break;
                    case DataType.SignedByte:
                        if (type == typeof(sbyte) || type == typeof(sbyte?))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            parameter.Value = Convert.ToSByte(value);
                        }
                        break;
                    case DataType.UnsignedShort:
                        if (type == typeof(ushort) || type == typeof(ushort?))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            parameter.Value = Convert.ToUInt16(value);
                        }
                        break;
                    case DataType.UnsignedInteger:
                        if (type == typeof(uint) || type == typeof(uint?))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            parameter.Value = Convert.ToUInt32(value);
                        }
                        break;
                    case DataType.UnsignedLong:
                        if (type == typeof(ulong) || type == typeof(ulong?))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            parameter.Value = Convert.ToUInt64(value);
                        }
                        break;
                    case DataType.ClrType:
                        parameter.Value = value;
                        break;
                    case DataType.ClrEnum:
                        parameter.Value = value;
                        break;
                    case DataType.Xml:
                        parameter.Value = value;
                        break;
                    case DataType.Json:
                        parameter.Value = value;
                        break;
                    case DataType.Chars:
                        if (type == typeof(char[]))
                        {
                            parameter.Value = value;
                        }
                        else
                        {
                            parameter.Value = (char[])value;
                        }
                        break;
                    case DataType.Undefined:
                    default:
                        throw new InvalidCastException($"Unsupported parameter type of Engine Command {dataType}");
                }
            }
            this.Parameters.Add(parameter.Name, parameter);
        }
    }
}
