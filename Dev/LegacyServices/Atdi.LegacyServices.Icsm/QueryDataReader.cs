using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Atdi.Common;
using Atdi.DataModels;

namespace Atdi.LegacyServices.Icsm
{
    internal sealed class QueryDataReader : Atdi.Contracts.CoreServices.DataLayer.IDataReader
    {
        private readonly System.Data.IDataReader _dataReader;
        private readonly IReadOnlyDictionary<string, string> _columnsMapper;

        public QueryDataReader(System.Data.IDataReader dataReader, IReadOnlyDictionary<string, string> columnsMapper)
        {
            this._dataReader = dataReader;
            this._columnsMapper = columnsMapper;
        }

        public object GetValueAsObject(DataType columnType, Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }

            switch (columnType)
            {
                case DataModels.DataType.Guid:
                    return this.GetValueAsGuid(fieldDbType, ordinal);
                case DataModels.DataType.String:
                    if (fieldDbType == typeof(Guid))
                         return this.GetValueAsGuid(fieldDbType, ordinal);
                    else return this.GetValueAsString(fieldDbType, ordinal);
                case DataModels.DataType.Boolean:
                    return this.GetValueAsBoolean(fieldDbType, ordinal);
                case DataModels.DataType.Integer:
                    return this.GetValueAsInt32(fieldDbType, ordinal);
                case DataModels.DataType.DateTime:
                    return this.GetValueAsDateTime(fieldDbType, ordinal);
                case DataModels.DataType.Double:
                    return this.GetValueAsDouble(fieldDbType, ordinal);
                case DataModels.DataType.Float:
                    return this.GetValueAsFloat(fieldDbType, ordinal);
                case DataModels.DataType.Decimal:
                    return this.GetValueAsDecimal(fieldDbType, ordinal);
                case DataModels.DataType.Byte:
                    return this.GetValueAsByte(fieldDbType, ordinal);
                case DataModels.DataType.Bytes:
                    return this.GetValueAsBytes(fieldDbType, ordinal);
                default:
                    throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(ordinal)));
            }
        }

        public string GetValueAsString(DataType columnType, Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }

            switch (columnType)
            {
                case DataModels.DataType.String:
                    if (fieldDbType == typeof(Guid))
                        return this.GetValueAsGuid(fieldDbType, ordinal).ToString();
                    else return this.GetValueAsString(fieldDbType, ordinal);
                case DataModels.DataType.Boolean:
                    return this.GetValueAsBoolean(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case DataModels.DataType.Integer:
                    return this.GetValueAsInt32(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case DataModels.DataType.DateTime:
                    return this.GetValueAsDateTime(fieldDbType, ordinal).ConvertToISO8601DateTimeString();
                case DataModels.DataType.Double:
                    return this.GetValueAsDouble(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case DataModels.DataType.Float:
                    return this.GetValueAsFloat(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case DataModels.DataType.Decimal:
                    return this.GetValueAsDecimal(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case DataModels.DataType.Byte:
                    return this.GetValueAsByte(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case DataModels.DataType.Bytes:
                    return UTF8Encoding.UTF8.GetString(this.GetValueAsBytes(fieldDbType, ordinal));
                default:
                    throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(ordinal)));
            }
        }

        public bool? GetNullableValueAsBoolean(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsBoolean(fieldDbType, ordinal);
        }

        public bool GetValueAsBoolean(Type fieldDbType, int ordinal)
        {
            if (fieldDbType == typeof(bool))
            {
                return _dataReader.GetBoolean(ordinal);
            }
            if (fieldDbType == typeof(int))
            {
                return Convert.ToBoolean(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToBoolean(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToBoolean(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToBoolean(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToBoolean(_dataReader.GetInt64(ordinal));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public int? GetNullableValueAsInt32(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsInt32(fieldDbType, ordinal);
        }

        public int GetValueAsInt32(Type fieldDbType, int ordinal)
        {
            if (fieldDbType == typeof(int))
            {
                return _dataReader.GetInt32(ordinal);
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToInt32(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToInt32(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToInt32(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToInt32(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToInt32(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToInt32(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(DateTime))
            {
                return Convert.ToInt32(_dataReader.GetDateTime(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToInt32(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToInt32(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToInt32(_dataReader.GetInt64(ordinal));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public float? GetNullableValueAsFloat(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsFloat(fieldDbType, ordinal);
        }

        public float GetValueAsFloat(Type fieldDbType, int ordinal)
        {
            if (fieldDbType == typeof(float))
            {
                return _dataReader.GetFloat(ordinal);
            }
            if (fieldDbType == typeof(string))
            {
                return (float)Convert.ToDouble(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return (float)Convert.ToDouble(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return (float)_dataReader.GetDouble(ordinal);
            }
            if (fieldDbType == typeof(int))
            {
                return (float)Convert.ToDouble(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return (float)Convert.ToDouble(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(DateTime))
            {
                return (float)Convert.ToDouble(_dataReader.GetDateTime(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return (float)Convert.ToDouble(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return (float)Convert.ToDouble(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return (float)Convert.ToDouble(_dataReader.GetInt64(ordinal));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public double? GetNullableValueAsDouble(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsDouble(fieldDbType, ordinal);
        }

        public double GetValueAsDouble(Type fieldDbType, int ordinal)
        {
            if (fieldDbType == typeof(double))
            {
                return _dataReader.GetDouble(ordinal);
            }
            if (fieldDbType == typeof(float))
            {
                return _dataReader.GetFloat(ordinal);
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToDouble(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToDouble(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(int))
            {
                return Convert.ToDouble(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToDouble(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(DateTime))
            {
                return Convert.ToDouble(_dataReader.GetDateTime(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToDouble(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToDouble(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToDouble(_dataReader.GetInt64(ordinal));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public decimal? GetNullableValueAsDecimal(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsDecimal(fieldDbType, ordinal);
        }

        public decimal GetValueAsDecimal(Type fieldDbType, int ordinal)
        {
            if (fieldDbType == typeof(decimal))
            {
                return _dataReader.GetDecimal(ordinal);
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToDecimal(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToDecimal(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToDecimal(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(int))
            {
                return Convert.ToDecimal(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToDecimal(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(DateTime))
            {
                return Convert.ToDecimal(_dataReader.GetDateTime(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToDecimal(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToDecimal(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToDecimal(_dataReader.GetInt64(ordinal));
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public string GetNullableValueAsString(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsString(fieldDbType, ordinal);
        }
 

        public string GetValueAsString(Type fieldDbType, int ordinal)
        {
            if (fieldDbType == typeof(string))
            {
                return _dataReader.GetString(ordinal);
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToString(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToString(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToString(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(int))
            {
                return Convert.ToString(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToString(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(DateTime))
            {
                return Convert.ToString(_dataReader.GetDateTime(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToString(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToString(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToString(_dataReader.GetInt64(ordinal));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public DateTime? GetNullableValueAsDateTime(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsDateTime(fieldDbType, ordinal);
        }

        public DateTime GetValueAsDateTime(Type fieldDbType, int ordinal)
        {
            if (fieldDbType == typeof(DateTime))
            {
                return _dataReader.GetDateTime(ordinal);
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToDateTime(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToDateTime(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToDateTime(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(int))
            {
                return Convert.ToDateTime(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToDateTime(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToDateTime(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToDateTime(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToDateTime(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToDateTime(_dataReader.GetInt64(ordinal));
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public byte? GetNullableValueAsByte(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsByte(fieldDbType, ordinal);
        }

        public byte GetValueAsByte(Type fieldDbType, int ordinal)
        {
            if (fieldDbType == typeof(byte))
            {
                return _dataReader.GetByte(ordinal);
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToByte(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToByte(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToByte(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(int))
            {
                return Convert.ToByte(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(DateTime))
            {
                return Convert.ToByte(_dataReader.GetDateTime(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToByte(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToByte(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToByte(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToByte(_dataReader.GetInt64(ordinal));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public Guid? GetNullableValueAsGuid(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsGuid(fieldDbType, ordinal);
        }

        public Guid GetValueAsGuid(Type fieldDbType, int ordinal)
        {
            if (fieldDbType == typeof(Guid))
            {
                return _dataReader.GetGuid(ordinal);
            }
            if (fieldDbType == typeof(string))
            {
                return Guid.Parse(_dataReader.GetString(ordinal));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public byte[] GetNullableValueAsBytes(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsBytes(fieldDbType, ordinal);
        }

        public byte[] GetValueAsBytes(Type fieldDbType, int ordinal)
        {
            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1024;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }

                return result;
            }
            if (fieldDbType == typeof(byte))
            {
                return new byte[] { _dataReader.GetByte(ordinal) };
            }
            if (fieldDbType == typeof(string))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetString(ordinal)) };
            }
            if (fieldDbType == typeof(double))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetDouble(ordinal)) };
            }
            if (fieldDbType == typeof(float))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetFloat(ordinal)) };
            }
            if (fieldDbType == typeof(int))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetInt32(ordinal)) };
            }
            if (fieldDbType == typeof(DateTime))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetDateTime(ordinal)) };
            }
            if (fieldDbType == typeof(decimal))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetDecimal(ordinal)) };
            }
            if (fieldDbType == typeof(Guid))
            {
                return _dataReader.GetGuid(ordinal).ToByteArray();
            }
            if (fieldDbType == typeof(char))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetChar(ordinal)) };
            }
            if (fieldDbType == typeof(short))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetInt16(ordinal)) };
            }
            if (fieldDbType == typeof(long))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetInt64(ordinal)) };
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public bool Read()
        {
            return this._dataReader.Read();
        }

        public int GetOrdinal(string name)
        {
            return this._dataReader.GetOrdinal(_columnsMapper[name]);
        }

        public Type GetFieldType(int ordinal)
        {
            return this._dataReader.GetFieldType(ordinal);
        }

        public bool IsDBNull(int ordinal)
        {
            return this._dataReader.IsDBNull(ordinal);
        }
    }
    internal sealed class QueryDataReader<TModel> : IDataReader<TModel>
    {
        private readonly System.Data.IDataReader _dataReader;
        private readonly IReadOnlyDictionary<string, string> _columnsMapper;
        public QueryDataReader(System.Data.IDataReader dataReader, IReadOnlyDictionary<string, string> columnsMapper)
        {
            this._dataReader = dataReader;
            this._columnsMapper = columnsMapper;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool GetValueAsBoolean(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(bool))
            {
                return _dataReader.GetBoolean(columnIndex);
            }
            if (columnType == typeof(int))
            {
                return Convert.ToBoolean(_dataReader.GetInt32(columnIndex));
            }
            if (columnType == typeof(string))
            {
                return Convert.ToBoolean(_dataReader.GetString(columnIndex));
            }
            if (columnType == typeof(char))
            {
                return Convert.ToBoolean(_dataReader.GetChar(columnIndex));
            }
            if (columnType == typeof(short))
            {
                return Convert.ToBoolean(_dataReader.GetInt16(columnIndex));
            }
            if (columnType == typeof(long))
            {
                return Convert.ToBoolean(_dataReader.GetInt64(columnIndex));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetValueAsInt32(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(int))
            {
                return _dataReader.GetInt32(columnIndex);
            }
            if (columnType == typeof(string))
            {
                return Convert.ToInt32(_dataReader.GetString(columnIndex));
            }
            if (columnType == typeof(decimal))
            {
                return Convert.ToInt32(_dataReader.GetDecimal(columnIndex));
            }
            if (columnType == typeof(double))
            {
                return Convert.ToInt32(_dataReader.GetDouble(columnIndex));
            }
            if (columnType == typeof(float))
            {
                return Convert.ToInt32(_dataReader.GetFloat(columnIndex));
            }
            if (columnType == typeof(byte))
            {
                return Convert.ToInt32(_dataReader.GetByte(columnIndex));
            }
            if (columnType == typeof(DateTime))
            {
                return Convert.ToInt32(_dataReader.GetDateTime(columnIndex));
            }
            if (columnType == typeof(char))
            {
                return Convert.ToInt32(_dataReader.GetChar(columnIndex));
            }
            if (columnType == typeof(short))
            {
                return Convert.ToInt32(_dataReader.GetInt16(columnIndex));
            }
            if (columnType == typeof(long))
            {
                return Convert.ToInt32(_dataReader.GetInt64(columnIndex));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetValueAsFloat(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(float))
            {
                return _dataReader.GetFloat(columnIndex);
            }
            if (columnType == typeof(string))
            {
                return (float)Convert.ToDouble(_dataReader.GetString(columnIndex));
            }
            if (columnType == typeof(decimal))
            {
                return (float)Convert.ToDouble(_dataReader.GetDecimal(columnIndex));
            }
            if (columnType == typeof(double))
            {
                return (float)_dataReader.GetDouble(columnIndex);
            }
            if (columnType == typeof(int))
            {
                return (float)Convert.ToDouble(_dataReader.GetInt32(columnIndex));
            }
            if (columnType == typeof(byte))
            {
                return (float)Convert.ToDouble(_dataReader.GetByte(columnIndex));
            }
            if (columnType == typeof(DateTime))
            {
                return (float)Convert.ToDouble(_dataReader.GetDateTime(columnIndex));
            }
            if (columnType == typeof(char))
            {
                return (float)Convert.ToDouble(_dataReader.GetChar(columnIndex));
            }
            if (columnType == typeof(short))
            {
                return (float)Convert.ToDouble(_dataReader.GetInt16(columnIndex));
            }
            if (columnType == typeof(long))
            {
                return (float)Convert.ToDouble(_dataReader.GetInt64(columnIndex));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double GetValueAsDouble(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(double))
            {
                return _dataReader.GetDouble(columnIndex);
            }
            if (columnType == typeof(float))
            {
                return _dataReader.GetFloat(columnIndex);
            }
            if (columnType == typeof(string))
            {
                return Convert.ToDouble(_dataReader.GetString(columnIndex));
            }
            if (columnType == typeof(decimal))
            {
                return Convert.ToDouble(_dataReader.GetDecimal(columnIndex));
            }
            if (columnType == typeof(int))
            {
                return Convert.ToDouble(_dataReader.GetInt32(columnIndex));
            }
            if (columnType == typeof(byte))
            {
                return Convert.ToDouble(_dataReader.GetByte(columnIndex));
            }
            if (columnType == typeof(DateTime))
            {
                return Convert.ToDouble(_dataReader.GetDateTime(columnIndex));
            }
            if (columnType == typeof(char))
            {
                return Convert.ToDouble(_dataReader.GetChar(columnIndex));
            }
            if (columnType == typeof(short))
            {
                return Convert.ToDouble(_dataReader.GetInt16(columnIndex));
            }
            if (columnType == typeof(long))
            {
                return Convert.ToDouble(_dataReader.GetInt64(columnIndex));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private decimal GetValueAsDecimal(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(decimal))
            {
                return _dataReader.GetDecimal(columnIndex);
            }
            if (columnType == typeof(string))
            {
                return Convert.ToDecimal(_dataReader.GetString(columnIndex));
            }
            if (columnType == typeof(double))
            {
                return Convert.ToDecimal(_dataReader.GetDouble(columnIndex));
            }
            if (columnType == typeof(float))
            {
                return Convert.ToDecimal(_dataReader.GetFloat(columnIndex));
            }
            if (columnType == typeof(int))
            {
                return Convert.ToDecimal(_dataReader.GetInt32(columnIndex));
            }
            if (columnType == typeof(byte))
            {
                return Convert.ToDecimal(_dataReader.GetByte(columnIndex));
            }
            if (columnType == typeof(DateTime))
            {
                return Convert.ToDecimal(_dataReader.GetDateTime(columnIndex));
            }
            if (columnType == typeof(char))
            {
                return Convert.ToDecimal(_dataReader.GetChar(columnIndex));
            }
            if (columnType == typeof(short))
            {
                return Convert.ToDecimal(_dataReader.GetInt16(columnIndex));
            }
            if (columnType == typeof(long))
            {
                return Convert.ToDecimal(_dataReader.GetInt64(columnIndex));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetValueAsString(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(string))
            {
                return _dataReader.GetString(columnIndex);
            }
            if (columnType == typeof(double))
            {
                return Convert.ToString(_dataReader.GetDouble(columnIndex));
            }
            if (columnType == typeof(decimal))
            {
                return Convert.ToString(_dataReader.GetDecimal(columnIndex));
            }
            if (columnType == typeof(float))
            {
                return Convert.ToString(_dataReader.GetFloat(columnIndex));
            }
            if (columnType == typeof(int))
            {
                return Convert.ToString(_dataReader.GetInt32(columnIndex));
            }
            if (columnType == typeof(byte))
            {
                return Convert.ToString(_dataReader.GetByte(columnIndex));
            }
            if (columnType == typeof(DateTime))
            {
                return Convert.ToString(_dataReader.GetDateTime(columnIndex));
            }
            if (columnType == typeof(char))
            {
                return Convert.ToString(_dataReader.GetChar(columnIndex));
            }
            if (columnType == typeof(short))
            {
                return Convert.ToString(_dataReader.GetInt16(columnIndex));
            }
            if (columnType == typeof(long))
            {
                return Convert.ToString(_dataReader.GetInt64(columnIndex));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DateTime GetValueAsDateTime(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(DateTime))
            {
                return _dataReader.GetDateTime(columnIndex);
            }
            if (columnType == typeof(string))
            {
                return Convert.ToDateTime(_dataReader.GetString(columnIndex));
            }
            if (columnType == typeof(double))
            {
                return Convert.ToDateTime(_dataReader.GetDouble(columnIndex));
            }
            if (columnType == typeof(float))
            {
                return Convert.ToDateTime(_dataReader.GetFloat(columnIndex));
            }
            if (columnType == typeof(int))
            {
                return Convert.ToDateTime(_dataReader.GetInt32(columnIndex));
            }
            if (columnType == typeof(byte))
            {
                return Convert.ToDateTime(_dataReader.GetByte(columnIndex));
            }
            if (columnType == typeof(decimal))
            {
                return Convert.ToDateTime(_dataReader.GetDecimal(columnIndex));
            }
            if (columnType == typeof(char))
            {
                return Convert.ToDateTime(_dataReader.GetChar(columnIndex));
            }
            if (columnType == typeof(short))
            {
                return Convert.ToDateTime(_dataReader.GetInt16(columnIndex));
            }
            if (columnType == typeof(long))
            {
                return Convert.ToDateTime(_dataReader.GetInt64(columnIndex));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte GetValueAsByte(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(byte))
            {
                return _dataReader.GetByte(columnIndex);
            }
            if (columnType == typeof(string))
            {
                return Convert.ToByte(_dataReader.GetString(columnIndex));
            }
            if (columnType == typeof(double))
            {
                return Convert.ToByte(_dataReader.GetDouble(columnIndex));
            }
            if (columnType == typeof(float))
            {
                return Convert.ToByte(_dataReader.GetFloat(columnIndex));
            }
            if (columnType == typeof(int))
            {
                return Convert.ToByte(_dataReader.GetInt32(columnIndex));
            }
            if (columnType == typeof(DateTime))
            {
                return Convert.ToByte(_dataReader.GetDateTime(columnIndex));
            }
            if (columnType == typeof(decimal))
            {
                return Convert.ToByte(_dataReader.GetDecimal(columnIndex));
            }
            if (columnType == typeof(char))
            {
                return Convert.ToByte(_dataReader.GetChar(columnIndex));
            }
            if (columnType == typeof(short))
            {
                return Convert.ToByte(_dataReader.GetInt16(columnIndex));
            }
            if (columnType == typeof(long))
            {
                return Convert.ToByte(_dataReader.GetInt64(columnIndex));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte[] GetValueAsBytes(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(columnIndex, 0, null, 0, 0); 
                var result = new byte[size];
                const int lehght = 1024;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght<= size- readBytes)
                        readBytes += _dataReader.GetBytes(columnIndex, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(columnIndex, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }

                return result;
            }
            if (columnType == typeof(byte))
            {
                return new byte[] { _dataReader.GetByte(columnIndex) };
            }
            if (columnType == typeof(string))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetString(columnIndex)) };
            }
            if (columnType == typeof(double))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetDouble(columnIndex)) };
            }
            if (columnType == typeof(float))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetFloat(columnIndex)) };
            }
            if (columnType == typeof(int))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetInt32(columnIndex)) };
            }
            if (columnType == typeof(DateTime))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetDateTime(columnIndex)) };
            }
            if (columnType == typeof(decimal))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetDecimal(columnIndex)) };
            }
            if (columnType == typeof(Guid))
            {
                return _dataReader.GetGuid(columnIndex).ToByteArray();
            }
            if (columnType == typeof(char))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetChar(columnIndex)) };
            }
            if (columnType == typeof(short))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetInt16(columnIndex)) };
            }
            if (columnType == typeof(long))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetInt64(columnIndex)) };
            }
            
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Guid GetValueAsGuid(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(Guid))
            {
                return _dataReader.GetGuid(columnIndex);
            }
            if (columnType == typeof(string))
            {
                return Guid.Parse(_dataReader.GetString(columnIndex));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        public bool GetValue(Expression<Func<TModel, bool>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsBoolean(columnIndex);
        }

        public bool? GetValue(Expression<Func<TModel, bool?>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }

            return GetValueAsBoolean(columnIndex);
        }

        public int GetValue(Expression<Func<TModel, int>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsInt32(columnIndex);
        }

        public int? GetValue(Expression<Func<TModel, int?>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }

            return GetValueAsInt32(columnIndex);
        }

        public float GetValue(Expression<Func<TModel, float>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsFloat(columnIndex);
        }

        public float? GetValue(Expression<Func<TModel, float?>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }

            return GetValueAsFloat(columnIndex);
        }

        public double GetValue(Expression<Func<TModel, double>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsDouble(columnIndex);
        }

        public double? GetValue(Expression<Func<TModel, double?>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }

            return GetValueAsDouble(columnIndex);
        }

        public decimal GetValue(Expression<Func<TModel, decimal>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsDecimal(columnIndex);
        }

        public decimal? GetValue(Expression<Func<TModel, decimal?>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }

            return GetValueAsDecimal(columnIndex);
        }

        public DateTime GetValue(Expression<Func<TModel, DateTime>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsDateTime(columnIndex);
        }

        public DateTime? GetValue(Expression<Func<TModel, DateTime?>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }

            return GetValueAsDateTime(columnIndex);
        }

        public string GetValue(Expression<Func<TModel, string>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);

            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }

            return GetValueAsString(columnIndex);
        }

        public byte GetValue(Expression<Func<TModel, byte>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsByte(columnIndex);
        }

        public byte? GetValue(Expression<Func<TModel, byte?>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }

            return GetValueAsByte(columnIndex);
        }

        public Guid GetValue(Expression<Func<TModel, Guid>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsGuid(columnIndex);
        }

        public Guid? GetValue(Expression<Func<TModel, Guid?>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }

            return GetValueAsGuid(columnIndex);
        }

        public bool Read()
        {
            return this._dataReader.Read();
        }

        public byte[] GetValue(Expression<Func<TModel, byte[]>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }
            return GetValueAsBytes(columnIndex);
        }
    }

    //public static class DataReaderExtenstions
    //{
    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static object GetValueAsObject(this IDataReader dataReader, DataModels.DataType columnType, Type fieldDbType, int ordinal)
    //    {
    //        if (dataReader.IsDBNull(ordinal))
    //        {
    //            return null;
    //        }

    //        switch (columnType)
    //        {
    //            case DataModels.DataType.String:
    //                return dataReader.GetValueAsString(fieldDbType, ordinal);
    //            case DataModels.DataType.Boolean:
    //                return dataReader.GetValueAsBoolean(fieldDbType, ordinal);
    //            case DataModels.DataType.Integer:
    //                return dataReader.GetValueAsInt32(fieldDbType, ordinal);
    //            case DataModels.DataType.DateTime:
    //                return dataReader.GetValueAsDateTime(fieldDbType, ordinal);
    //            case DataModels.DataType.Double:
    //                return dataReader.GetValueAsDouble(fieldDbType, ordinal);
    //            case DataModels.DataType.Float:
    //                return dataReader.GetValueAsFloat(fieldDbType, ordinal);
    //            case DataModels.DataType.Decimal:
    //                return dataReader.GetValueAsDecimal(fieldDbType, ordinal);
    //            case DataModels.DataType.Byte:
    //                return dataReader.GetValueAsByte(fieldDbType, ordinal);
    //            case DataModels.DataType.Bytes:
    //                return dataReader.GetValueAsBytes(fieldDbType, ordinal);
    //            default:
    //                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, dataReader.GetName(ordinal)));
    //        }
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static string GetValueAsString(this IDataReader dataReader, DataModels.DataType columnType, Type fieldDbType, int ordinal)
    //    {
    //        if (dataReader.IsDBNull(ordinal))
    //        {
    //            return null;
    //        }

    //        switch (columnType)
    //        {
    //            case DataModels.DataType.String:
    //                return dataReader.GetValueAsString(fieldDbType, ordinal);
    //            case DataModels.DataType.Boolean:
    //                return dataReader.GetValueAsBoolean(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
    //            case DataModels.DataType.Integer:
    //                return dataReader.GetValueAsInt32(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
    //            case DataModels.DataType.DateTime:
    //                return dataReader.GetValueAsDateTime(fieldDbType, ordinal).ConvertToISO8601DateTimeString();
    //            case DataModels.DataType.Double:
    //                return dataReader.GetValueAsDouble(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
    //            case DataModels.DataType.Float:
    //                return dataReader.GetValueAsFloat(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
    //            case DataModels.DataType.Decimal:
    //                return dataReader.GetValueAsDecimal(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
    //            case DataModels.DataType.Byte:
    //                return dataReader.GetValueAsByte(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
    //            case DataModels.DataType.Bytes:
    //                return UTF8Encoding.UTF8.GetString(dataReader.GetValueAsBytes(fieldDbType, ordinal));
    //            default:
    //                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, dataReader.GetName(ordinal)));
    //        }
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static bool? GetNullableValueAsBoolean(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (dataReader.IsDBNull(ordinal))
    //        {
    //            return null;
    //        }
    //        return dataReader.GetValueAsBoolean(fieldDbType, ordinal);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static bool GetValueAsBoolean(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (fieldDbType == typeof(bool))
    //        {
    //            return dataReader.GetBoolean(ordinal);
    //        }
    //        if (fieldDbType == typeof(int))
    //        {
    //            return Convert.ToBoolean(dataReader.GetInt32(ordinal));
    //        }
    //        if (fieldDbType == typeof(string))
    //        {
    //            return Convert.ToBoolean(dataReader.GetString(ordinal));
    //        }
    //        if (fieldDbType == typeof(char))
    //        {
    //            return Convert.ToBoolean(dataReader.GetChar(ordinal));
    //        }
    //        if (fieldDbType == typeof(short))
    //        {
    //            return Convert.ToBoolean(dataReader.GetInt16(ordinal));
    //        }
    //        if (fieldDbType == typeof(long))
    //        {
    //            return Convert.ToBoolean(dataReader.GetInt64(ordinal));
    //        }
    //        throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetName(ordinal)));
    //    }


    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static int? GetNullableValueAsInt32(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (dataReader.IsDBNull(ordinal))
    //        {
    //            return null;
    //        }
    //        return dataReader.GetValueAsInt32(fieldDbType, ordinal);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static int GetValueAsInt32(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (fieldDbType == typeof(int))
    //        {
    //            return dataReader.GetInt32(ordinal);
    //        }
    //        if (fieldDbType == typeof(string))
    //        {
    //            return Convert.ToInt32(dataReader.GetString(ordinal));
    //        }
    //        if (fieldDbType == typeof(decimal))
    //        {
    //            return Convert.ToInt32(dataReader.GetDecimal(ordinal));
    //        }
    //        if (fieldDbType == typeof(double))
    //        {
    //            return Convert.ToInt32(dataReader.GetDouble(ordinal));
    //        }
    //        if (fieldDbType == typeof(float))
    //        {
    //            return Convert.ToInt32(dataReader.GetFloat(ordinal));
    //        }
    //        if (fieldDbType == typeof(byte))
    //        {
    //            return Convert.ToInt32(dataReader.GetByte(ordinal));
    //        }
    //        if (fieldDbType == typeof(DateTime))
    //        {
    //            return Convert.ToInt32(dataReader.GetDateTime(ordinal));
    //        }
    //        if (fieldDbType == typeof(char))
    //        {
    //            return Convert.ToInt32(dataReader.GetChar(ordinal));
    //        }
    //        if (fieldDbType == typeof(short))
    //        {
    //            return Convert.ToInt32(dataReader.GetInt16(ordinal));
    //        }
    //        if (fieldDbType == typeof(long))
    //        {
    //            return Convert.ToInt32(dataReader.GetInt64(ordinal));
    //        }
    //        throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetName(ordinal)));
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static float? GetNullableValueAsFloat(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (dataReader.IsDBNull(ordinal))
    //        {
    //            return null;
    //        }
    //        return dataReader.GetValueAsFloat(fieldDbType, ordinal);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static float GetValueAsFloat(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (fieldDbType == typeof(float))
    //        {
    //            return dataReader.GetFloat(ordinal);
    //        }
    //        if (fieldDbType == typeof(string))
    //        {
    //            return (float)Convert.ToDouble(dataReader.GetString(ordinal));
    //        }
    //        if (fieldDbType == typeof(decimal))
    //        {
    //            return (float)Convert.ToDouble(dataReader.GetDecimal(ordinal));
    //        }
    //        if (fieldDbType == typeof(double))
    //        {
    //            return (float)dataReader.GetDouble(ordinal);
    //        }
    //        if (fieldDbType == typeof(int))
    //        {
    //            return (float)Convert.ToDouble(dataReader.GetInt32(ordinal));
    //        }
    //        if (fieldDbType == typeof(byte))
    //        {
    //            return (float)Convert.ToDouble(dataReader.GetByte(ordinal));
    //        }
    //        if (fieldDbType == typeof(DateTime))
    //        {
    //            return (float)Convert.ToDouble(dataReader.GetDateTime(ordinal));
    //        }
    //        if (fieldDbType == typeof(char))
    //        {
    //            return (float)Convert.ToDouble(dataReader.GetChar(ordinal));
    //        }
    //        if (fieldDbType == typeof(short))
    //        {
    //            return (float)Convert.ToDouble(dataReader.GetInt16(ordinal));
    //        }
    //        if (fieldDbType == typeof(long))
    //        {
    //            return (float)Convert.ToDouble(dataReader.GetInt64(ordinal));
    //        }
    //        throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetName(ordinal)));
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static double? GetNullableValueAsDouble(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (dataReader.IsDBNull(ordinal))
    //        {
    //            return null;
    //        }
    //        return dataReader.GetValueAsDouble(fieldDbType, ordinal);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static double GetValueAsDouble(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (fieldDbType == typeof(double))
    //        {
    //            return dataReader.GetDouble(ordinal);
    //        }
    //        if (fieldDbType == typeof(float))
    //        {
    //            return dataReader.GetFloat(ordinal);
    //        }
    //        if (fieldDbType == typeof(string))
    //        {
    //            return Convert.ToDouble(dataReader.GetString(ordinal));
    //        }
    //        if (fieldDbType == typeof(decimal))
    //        {
    //            return Convert.ToDouble(dataReader.GetDecimal(ordinal));
    //        }
    //        if (fieldDbType == typeof(int))
    //        {
    //            return Convert.ToDouble(dataReader.GetInt32(ordinal));
    //        }
    //        if (fieldDbType == typeof(byte))
    //        {
    //            return Convert.ToDouble(dataReader.GetByte(ordinal));
    //        }
    //        if (fieldDbType == typeof(DateTime))
    //        {
    //            return Convert.ToDouble(dataReader.GetDateTime(ordinal));
    //        }
    //        if (fieldDbType == typeof(char))
    //        {
    //            return Convert.ToDouble(dataReader.GetChar(ordinal));
    //        }
    //        if (fieldDbType == typeof(short))
    //        {
    //            return Convert.ToDouble(dataReader.GetInt16(ordinal));
    //        }
    //        if (fieldDbType == typeof(long))
    //        {
    //            return Convert.ToDouble(dataReader.GetInt64(ordinal));
    //        }
    //        throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetName(ordinal)));
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static decimal? GetNullableValueAsDecimal(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (dataReader.IsDBNull(ordinal))
    //        {
    //            return null;
    //        }
    //        return dataReader.GetValueAsDecimal(fieldDbType, ordinal);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static decimal GetValueAsDecimal(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (fieldDbType == typeof(decimal))
    //        {
    //            return dataReader.GetDecimal(ordinal);
    //        }
    //        if (fieldDbType == typeof(string))
    //        {
    //            return Convert.ToDecimal(dataReader.GetString(ordinal));
    //        }
    //        if (fieldDbType == typeof(double))
    //        {
    //            return Convert.ToDecimal(dataReader.GetDouble(ordinal));
    //        }
    //        if (fieldDbType == typeof(float))
    //        {
    //            return Convert.ToDecimal(dataReader.GetFloat(ordinal));
    //        }
    //        if (fieldDbType == typeof(int))
    //        {
    //            return Convert.ToDecimal(dataReader.GetInt32(ordinal));
    //        }
    //        if (fieldDbType == typeof(byte))
    //        {
    //            return Convert.ToDecimal(dataReader.GetByte(ordinal));
    //        }
    //        if (fieldDbType == typeof(DateTime))
    //        {
    //            return Convert.ToDecimal(dataReader.GetDateTime(ordinal));
    //        }
    //        if (fieldDbType == typeof(char))
    //        {
    //            return Convert.ToDecimal(dataReader.GetChar(ordinal));
    //        }
    //        if (fieldDbType == typeof(short))
    //        {
    //            return Convert.ToDecimal(dataReader.GetInt16(ordinal));
    //        }
    //        if (fieldDbType == typeof(long))
    //        {
    //            return Convert.ToDecimal(dataReader.GetInt64(ordinal));
    //        }

    //        throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetName(ordinal)));
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static string GetNullableValueAsString(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (dataReader.IsDBNull(ordinal))
    //        {
    //            return null;
    //        }
    //        return dataReader.GetValueAsString(fieldDbType, ordinal);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static string GetValueAsString(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (fieldDbType == typeof(string))
    //        {
    //            return dataReader.GetString(ordinal);
    //        }
    //        if (fieldDbType == typeof(double))
    //        {
    //            return Convert.ToString(dataReader.GetDouble(ordinal));
    //        }
    //        if (fieldDbType == typeof(decimal))
    //        {
    //            return Convert.ToString(dataReader.GetDecimal(ordinal));
    //        }
    //        if (fieldDbType == typeof(float))
    //        {
    //            return Convert.ToString(dataReader.GetFloat(ordinal));
    //        }
    //        if (fieldDbType == typeof(int))
    //        {
    //            return Convert.ToString(dataReader.GetInt32(ordinal));
    //        }
    //        if (fieldDbType == typeof(byte))
    //        {
    //            return Convert.ToString(dataReader.GetByte(ordinal));
    //        }
    //        if (fieldDbType == typeof(DateTime))
    //        {
    //            return Convert.ToString(dataReader.GetDateTime(ordinal));
    //        }
    //        if (fieldDbType == typeof(char))
    //        {
    //            return Convert.ToString(dataReader.GetChar(ordinal));
    //        }
    //        if (fieldDbType == typeof(short))
    //        {
    //            return Convert.ToString(dataReader.GetInt16(ordinal));
    //        }
    //        if (fieldDbType == typeof(long))
    //        {
    //            return Convert.ToString(dataReader.GetInt64(ordinal));
    //        }
    //        throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetName(ordinal)));
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static DateTime? GetNullableValueAsDateTime(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (dataReader.IsDBNull(ordinal))
    //        {
    //            return null;
    //        }
    //        return dataReader.GetValueAsDateTime(fieldDbType, ordinal);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static DateTime GetValueAsDateTime(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (fieldDbType == typeof(DateTime))
    //        {
    //            return dataReader.GetDateTime(ordinal);
    //        }
    //        if (fieldDbType == typeof(string))
    //        {
    //            return Convert.ToDateTime(dataReader.GetString(ordinal));
    //        }
    //        if (fieldDbType == typeof(double))
    //        {
    //            return Convert.ToDateTime(dataReader.GetDouble(ordinal));
    //        }
    //        if (fieldDbType == typeof(float))
    //        {
    //            return Convert.ToDateTime(dataReader.GetFloat(ordinal));
    //        }
    //        if (fieldDbType == typeof(int))
    //        {
    //            return Convert.ToDateTime(dataReader.GetInt32(ordinal));
    //        }
    //        if (fieldDbType == typeof(byte))
    //        {
    //            return Convert.ToDateTime(dataReader.GetByte(ordinal));
    //        }
    //        if (fieldDbType == typeof(decimal))
    //        {
    //            return Convert.ToDateTime(dataReader.GetDecimal(ordinal));
    //        }
    //        if (fieldDbType == typeof(char))
    //        {
    //            return Convert.ToDateTime(dataReader.GetChar(ordinal));
    //        }
    //        if (fieldDbType == typeof(short))
    //        {
    //            return Convert.ToDateTime(dataReader.GetInt16(ordinal));
    //        }
    //        if (fieldDbType == typeof(long))
    //        {
    //            return Convert.ToDateTime(dataReader.GetInt64(ordinal));
    //        }

    //        throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetName(ordinal)));
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static byte? GetNullableValueAsByte(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (dataReader.IsDBNull(ordinal))
    //        {
    //            return null;
    //        }
    //        return dataReader.GetValueAsByte(fieldDbType, ordinal);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static byte GetValueAsByte(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (fieldDbType == typeof(byte))
    //        {
    //            return dataReader.GetByte(ordinal);
    //        }
    //        if (fieldDbType == typeof(string))
    //        {
    //            return Convert.ToByte(dataReader.GetString(ordinal));
    //        }
    //        if (fieldDbType == typeof(double))
    //        {
    //            return Convert.ToByte(dataReader.GetDouble(ordinal));
    //        }
    //        if (fieldDbType == typeof(float))
    //        {
    //            return Convert.ToByte(dataReader.GetFloat(ordinal));
    //        }
    //        if (fieldDbType == typeof(int))
    //        {
    //            return Convert.ToByte(dataReader.GetInt32(ordinal));
    //        }
    //        if (fieldDbType == typeof(DateTime))
    //        {
    //            return Convert.ToByte(dataReader.GetDateTime(ordinal));
    //        }
    //        if (fieldDbType == typeof(decimal))
    //        {
    //            return Convert.ToByte(dataReader.GetDecimal(ordinal));
    //        }
    //        if (fieldDbType == typeof(char))
    //        {
    //            return Convert.ToByte(dataReader.GetChar(ordinal));
    //        }
    //        if (fieldDbType == typeof(short))
    //        {
    //            return Convert.ToByte(dataReader.GetInt16(ordinal));
    //        }
    //        if (fieldDbType == typeof(long))
    //        {
    //            return Convert.ToByte(dataReader.GetInt64(ordinal));
    //        }
    //        throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetName(ordinal)));
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static Guid? GetNullableValueAsGuid(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (dataReader.IsDBNull(ordinal))
    //        {
    //            return null;
    //        }
    //        return dataReader.GetValueAsGuid(fieldDbType, ordinal);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static Guid GetValueAsGuid(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (fieldDbType == typeof(Guid))
    //        {
    //            return dataReader.GetGuid(ordinal);
    //        }
    //        if (fieldDbType == typeof(string))
    //        {
    //            return Guid.Parse(dataReader.GetString(ordinal));
    //        }
    //        throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetName(ordinal)));
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static byte[] GetNullableValueAsBytes(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (dataReader.IsDBNull(ordinal))
    //        {
    //            return null;
    //        }
    //        return dataReader.GetValueAsBytes(fieldDbType, ordinal);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static byte[] GetValueAsBytes(this IDataReader dataReader, Type fieldDbType, int ordinal)
    //    {
    //        if (fieldDbType == typeof(byte[]))
    //        {
    //            var size = dataReader.GetBytes(ordinal, 0, null, 0, 0);
    //            var result = new byte[size];
    //            const int lehght = 1024;
    //            long readBytes = 0;
    //            int offset = 0;
    //            while (readBytes < size)
    //            {
    //                if (lehght <= size - readBytes)
    //                    readBytes += dataReader.GetBytes(ordinal, offset, result, offset, lehght);
    //                else readBytes += dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

    //                offset += lehght;
    //            }

    //            return result;
    //        }
    //        if (fieldDbType == typeof(byte))
    //        {
    //            return new byte[] { dataReader.GetByte(ordinal) };
    //        }
    //        if (fieldDbType == typeof(string))
    //        {
    //            return new byte[] { Convert.ToByte(dataReader.GetString(ordinal)) };
    //        }
    //        if (fieldDbType == typeof(double))
    //        {
    //            return new byte[] { Convert.ToByte(dataReader.GetDouble(ordinal)) };
    //        }
    //        if (fieldDbType == typeof(float))
    //        {
    //            return new byte[] { Convert.ToByte(dataReader.GetFloat(ordinal)) };
    //        }
    //        if (fieldDbType == typeof(int))
    //        {
    //            return new byte[] { Convert.ToByte(dataReader.GetInt32(ordinal)) };
    //        }
    //        if (fieldDbType == typeof(DateTime))
    //        {
    //            return new byte[] { Convert.ToByte(dataReader.GetDateTime(ordinal)) };
    //        }
    //        if (fieldDbType == typeof(decimal))
    //        {
    //            return new byte[] { Convert.ToByte(dataReader.GetDecimal(ordinal)) };
    //        }
    //        if (fieldDbType == typeof(Guid))
    //        {
    //            return dataReader.GetGuid(ordinal).ToByteArray();
    //        }
    //        if (fieldDbType == typeof(char))
    //        {
    //            return new byte[] { Convert.ToByte(dataReader.GetChar(ordinal)) };
    //        }
    //        if (fieldDbType == typeof(short))
    //        {
    //            return new byte[] { Convert.ToByte(dataReader.GetInt16(ordinal)) };
    //        }
    //        if (fieldDbType == typeof(long))
    //        {
    //            return new byte[] { Convert.ToByte(dataReader.GetInt64(ordinal)) };
    //        }
            
    //        throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetName(ordinal)));
    //    }
    //}
}
