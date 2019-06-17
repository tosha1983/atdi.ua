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
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class QueryDataReader : Atdi.Contracts.CoreServices.DataLayer.IDataReader
    {
        private readonly System.Data.IDataReader _dataReader;
        private readonly IReadOnlyDictionary<string, string> _columnsMapper;
        private readonly IDataTypeMetadata[] _fieldTypeMetadatas;
        private readonly DataTypeSystem _dataTypeSystem;

        public QueryDataReader(System.Data.IDataReader dataReader, IReadOnlyDictionary<string, string> columnsMapper, IDataTypeMetadata[] fieldTypeMetadatas, DataTypeSystem dataTypeSystem)
        {
            this._dataReader = dataReader;
            this._columnsMapper = columnsMapper;
            this._fieldTypeMetadatas = fieldTypeMetadatas;
            this._dataTypeSystem = dataTypeSystem;
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
                case DataModels.DataType.Char:
                    return this.GetValueAsChar(fieldDbType, ordinal);
                case DataModels.DataType.Short:
                    return this.GetValueAsShort(fieldDbType, ordinal);
                case DataModels.DataType.UnsignedShort:
                    return this.GetValueAsUnsignedShort(fieldDbType, ordinal);
                case DataModels.DataType.UnsignedInteger:
                    return this.GetValueAsUnsignedInteger(fieldDbType, ordinal);
                case DataModels.DataType.Long:
                    return this.GetValueAsLong(fieldDbType, ordinal);
                case DataModels.DataType.UnsignedLong:
                    return this.GetValueAsUnsignedLong(fieldDbType, ordinal);
                case DataModels.DataType.SignedByte:
                    return this.GetValueAsSignedByte(fieldDbType, ordinal);
                case DataModels.DataType.Time:
                    return this.GetValueAsTime(fieldDbType, ordinal).ToString();
                case DataModels.DataType.Date:
                    return this.GetValueAsDate(fieldDbType, ordinal);
                case DataModels.DataType.DateTimeOffset:
                    return this.GetValueAsDateTimeOffset(fieldDbType, ordinal);
                case DataModels.DataType.Xml:
                    return this.GetValueAsXml(fieldDbType, ordinal);
                case DataModels.DataType.Json:
                    return this.GetValueAsJson(fieldDbType, ordinal);
                case DataModels.DataType.ClrEnum:
                    return this.GetValueAsClrEnum(fieldDbType, ordinal);
                case DataModels.DataType.ClrType:
                    return this.GetValueAsClrType(fieldDbType, ordinal);

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
                case DataModels.DataType.Char:
                    return this.GetValueAsChar(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case DataModels.DataType.Short:
                    return this.GetValueAsShort(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case DataModels.DataType.UnsignedShort:
                    return this.GetValueAsUnsignedShort(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case DataModels.DataType.UnsignedInteger:
                    return this.GetValueAsUnsignedInteger(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case DataModels.DataType.Long:
                    return this.GetValueAsLong(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case DataModels.DataType.UnsignedLong:
                    return this.GetValueAsUnsignedLong(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case DataModels.DataType.SignedByte:
                    return this.GetValueAsSignedByte(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case DataModels.DataType.Time:
                    return this.GetValueAsTime(fieldDbType, ordinal).ToString();
                case DataModels.DataType.Date:
                    return this.GetValueAsDate(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case DataModels.DataType.DateTimeOffset:
                    return this.GetValueAsDateTimeOffset(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case DataModels.DataType.Xml:
                    return this.GetValueAsXml(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case DataModels.DataType.Json:
                    return this.GetValueAsJson(fieldDbType, ordinal).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case DataModels.DataType.ClrEnum:
                    return this.GetValueAsClrEnum(fieldDbType, ordinal).ToString();
                case DataModels.DataType.ClrType:
                    return this.GetValueAsClrType(fieldDbType, ordinal).ToString();

                default:
                    throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(ordinal)));
            }
        }

        public char? GetNullableValueAsChar(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsChar(fieldDbType, ordinal);
        }

        public char GetValueAsChar(Type fieldDbType, int ordinal)
        {
            if (fieldDbType == typeof(int))
            {
                return Convert.ToChar(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToChar(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToChar(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToChar(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToChar(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToChar(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToChar(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToChar(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToChar(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(DateTime))
            {
                return Convert.ToChar(_dataReader.GetDateTime(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToChar(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToChar(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToChar(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToChar(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToChar(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(Guid))
            {
                return Convert.ToChar(_dataReader.GetGuid(ordinal));
            }

            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToChar(result[0]);
                }
            }
            if (fieldDbType == typeof(TimeSpan))
            {
                return Convert.ToChar(DateTimeToTimeSpan(_dataReader.GetDateTime(ordinal)));
            }
            if (fieldDbType == typeof(Enum))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            if (fieldDbType == typeof(Object))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public short? GetNullableValueAsShort(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsShort(fieldDbType, ordinal);
        }

        public short GetValueAsShort(Type fieldDbType, int ordinal)
        {
            if (fieldDbType == typeof(int))
            {
                return Convert.ToInt16(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToInt16(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToInt16(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToInt16(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToInt16(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToInt16(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToInt16(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToInt16(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToInt16(_dataReader.GetByte(ordinal));
            }
         
            if (fieldDbType == typeof(char))
            {
                return Convert.ToInt16(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToInt16(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToInt16(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToInt16(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToInt16(_dataReader.GetInt64(ordinal));
            }

            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToInt16(result[0]);
                }
            }
            if (fieldDbType == typeof(TimeSpan))
            {
                return Convert.ToInt16(DateTimeToTimeSpan(_dataReader.GetDateTime(ordinal)));
            }
            if (fieldDbType == typeof(Enum))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            if (fieldDbType == typeof(Object))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public UInt16? GetNullableValueAsUnsignedShort(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsUnsignedShort(fieldDbType, ordinal);
        }


        public UInt16 GetValueAsUnsignedShort(Type fieldDbType, int ordinal)
        {
            if (fieldDbType == typeof(int))
            {
                return Convert.ToUInt16(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToUInt16(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToUInt16(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToUInt16(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToUInt16(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToUInt16(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToUInt16(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToUInt16(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToUInt16(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToUInt16(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToUInt16(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToUInt16(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToUInt16(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToUInt16(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToUInt16(result[0]);
                }
            }
            if (fieldDbType == typeof(TimeSpan))
            {
                return Convert.ToUInt16(DateTimeToTimeSpan(_dataReader.GetDateTime(ordinal)));
            }
            if (fieldDbType == typeof(Enum))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            if (fieldDbType == typeof(Object))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public UInt32? GetNullableValueAsUnsignedInteger(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsUnsignedInteger(fieldDbType, ordinal);
        }

        public UInt32 GetValueAsUnsignedInteger(Type fieldDbType, int ordinal)
        {
            if (fieldDbType == typeof(int))
            {
                return Convert.ToUInt32(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToUInt32(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToUInt32(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToUInt32(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToUInt32(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToUInt32(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToUInt32(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToUInt32(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToUInt32(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToUInt32(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToUInt32(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToUInt32(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToUInt32(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToUInt32(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToUInt32(result[0]);
                }
            }
            if (fieldDbType == typeof(TimeSpan))
            {
                return Convert.ToUInt32(DateTimeToTimeSpan(_dataReader.GetDateTime(ordinal)));
            }
            if (fieldDbType == typeof(Enum))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            if (fieldDbType == typeof(Object))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public long? GetNullableValueAsLong(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsLong(fieldDbType, ordinal);
        }

        public long GetValueAsLong(Type fieldDbType, int ordinal)
        {
            if (fieldDbType == typeof(int))
            {
                return Convert.ToInt64(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToInt64(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToInt64(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToInt64(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToInt64(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToInt64(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToInt64(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToInt64(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToInt64(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToInt64(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToInt64(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToInt64(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToInt64(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToInt64(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToInt64(result[0]);
                }
            }
            if (fieldDbType == typeof(TimeSpan))
            {
                return Convert.ToInt64(DateTimeToTimeSpan(_dataReader.GetDateTime(ordinal)));
            }
            if (fieldDbType == typeof(Enum))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            if (fieldDbType == typeof(Object))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public UInt64? GetNullableValueAsUnsignedLong(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsUnsignedLong(fieldDbType, ordinal);
        }

        public UInt64 GetValueAsUnsignedLong(Type fieldDbType, int ordinal)
        {
            if (fieldDbType == typeof(int))
            {
                return Convert.ToUInt64(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToUInt64(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToUInt64(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToUInt64(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToUInt64(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToUInt64(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToUInt64(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToUInt64(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToUInt64(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToUInt64(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToUInt64(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToUInt64(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToUInt64(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToUInt64(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToUInt64(result[0]);
                }
            }
            if (fieldDbType == typeof(TimeSpan))
            {
                return Convert.ToUInt64(DateTimeToTimeSpan(_dataReader.GetDateTime(ordinal)));
            }
            if (fieldDbType == typeof(Enum))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            if (fieldDbType == typeof(Object))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public sbyte? GetNullableValueAsSignedByte(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsSignedByte(fieldDbType, ordinal);
        }

        public sbyte GetValueAsSignedByte(Type fieldDbType, int ordinal)
        {
            if (fieldDbType == typeof(int))
            {
                return Convert.ToSByte(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToSByte(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToSByte(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToSByte(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToSByte(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToSByte(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToSByte(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToSByte(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToSByte(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToSByte(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToSByte(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToSByte(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToSByte(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToSByte(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToSByte(result[0]);
                }
            }
            if (fieldDbType == typeof(TimeSpan))
            {
                return Convert.ToSByte(DateTimeToTimeSpan(_dataReader.GetDateTime(ordinal)));
            }
            if (fieldDbType == typeof(Enum))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            if (fieldDbType == typeof(Object))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public TimeSpan? GetNullableValueAsTime(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsTime(fieldDbType, ordinal);
        }
        public TimeSpan GetValueAsTime(Type fieldDbType, int ordinal)
        {
            if (fieldDbType == typeof(DateTime))
            {
                return DateTimeToTimeSpan(Convert.ToDateTime(_dataReader.GetInt32(ordinal)));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public DateTime? GetNullableValueAsDate(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsDate(fieldDbType, ordinal);
        }

        public DateTime GetValueAsDate(Type fieldDbType, int ordinal)
        {

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public DateTimeOffset? GetNullableValueAsDateTimeOffset(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsDateTimeOffset(fieldDbType, ordinal);
        }

        public DateTimeOffset GetValueAsDateTimeOffset(Type fieldDbType, int ordinal)
        {

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public string GetNullableValueAsXml(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsXml(fieldDbType, ordinal);
        }

        public string GetValueAsXml(Type fieldDbType, int ordinal)
        {

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public string GetNullableValueAsJson(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsJson(fieldDbType, ordinal);
        }

        public string GetValueAsJson(Type fieldDbType, int ordinal)
        {

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public Enum GetNullableValueAsClrEnum(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsClrEnum(fieldDbType, ordinal);
        }

        public Enum GetValueAsClrEnum(Type fieldDbType, int ordinal)
        {

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public Object GetNullableValueAsClrType(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsClrType(fieldDbType, ordinal);
        }

        public Object GetValueAsClrType(Type fieldDbType, int ordinal)
        {

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public bool? GetNullableValueAsBoolean(Type fieldDbType, int ordinal)
        {
            if (_dataReader.IsDBNull(ordinal))
            {
                return null;
            }
            return this.GetValueAsBoolean(fieldDbType, ordinal);
        }

        private T GetInternalValue<T>(int ordinal)
        {
            var fieldTypeMetadata = _fieldTypeMetadatas[ordinal];
            return _dataTypeSystem.GetDecoder<T>(fieldTypeMetadata).DecodeAs(_dataReader, ordinal);
        }

        public bool GetValueAsBoolean(Type fieldDbType, int ordinal)
        {
            var value = GetInternalValue<bool?>(ordinal);
            return value.Value;
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
            if (fieldDbType == typeof(UInt32))
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
            if (fieldDbType == typeof(sbyte))
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
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToInt32(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToInt32(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToInt32(_dataReader.GetInt64(ordinal));
            }

            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToInt32(result[0]);
                }
            }
            if (fieldDbType == typeof(TimeSpan))
            {
                return Convert.ToInt32(DateTimeToTimeSpan(_dataReader.GetDateTime(ordinal)));
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public TimeSpan DateTimeToTimeSpan(DateTime? ts)
        {
            if (!ts.HasValue) return TimeSpan.Zero;
            else return new TimeSpan(0, ts.Value.Hour, ts.Value.Minute, ts.Value.Second, ts.Value.Millisecond);
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
            if (fieldDbType == typeof(bool))
            {
                return (float)Convert.ToSingle(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return _dataReader.GetFloat(ordinal);
            }
            if (fieldDbType == typeof(string))
            {
                return (float)Convert.ToSingle(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return (float)Convert.ToSingle(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return (float)_dataReader.GetDouble(ordinal);
            }
            if (fieldDbType == typeof(int))
            {
                return (float)Convert.ToSingle(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(UInt32))
            {
                return (float)Convert.ToSingle(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return (float)Convert.ToSingle(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return (float)Convert.ToSingle(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(ushort))
            {
                return (float)Convert.ToSingle(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return (float)Convert.ToSingle(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return (float)Convert.ToSingle(_dataReader.GetInt64(ordinal));
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
            if (fieldDbType == typeof(bool))
            {
                return (double)Convert.ToDouble(_dataReader.GetBoolean(ordinal));
            }
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
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToDouble(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToDouble(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToDouble(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToDouble(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToDouble(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
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
            if (fieldDbType == typeof(bool))
            {
                return (decimal)Convert.ToDecimal(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToDecimal(_dataReader.GetChar(ordinal));
            }
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
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToDecimal(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToDecimal(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToDecimal(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToDecimal(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
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
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToString(_dataReader.GetBoolean(ordinal));
            }
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
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToString(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToString(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
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
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToString(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToString(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToString(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(Guid))
            {
                return Convert.ToString(_dataReader.GetGuid(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToString(result[0]);
                }
            }
            if (fieldDbType == typeof(TimeSpan))
            {
                return Convert.ToString(_dataReader.GetDateTime(ordinal));
            }

            if (fieldDbType == typeof(DateTimeOffset))
            {
                return Convert.ToString(_dataReader.GetDateTime(ordinal));
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
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToDateTime(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToDateTime(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToDateTime(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToDateTime(_dataReader.GetDecimal(ordinal));
            }
         
            if (fieldDbType == typeof(short))
            {
                return Convert.ToDateTime(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToDateTime(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToDateTime(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToDateTime(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(DateTimeOffset))
            {
                return _dataReader.GetDateTime(ordinal);
            }
            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToDateTime(result[0]);
                }
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
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToByte(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return _dataReader.GetByte(ordinal);
            }
            if (fieldDbType == typeof(sbyte))
            {
                return _dataReader.GetByte(ordinal);
            }
            if (fieldDbType == typeof(Enum))
            {
                return Convert.ToByte(_dataReader.GetString(ordinal));
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
            if (fieldDbType == typeof(UInt32))
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
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToByte(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToByte(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToByte(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return result[0];
                }

                return default(byte);
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
            if (fieldDbType == typeof(sbyte))
            {
                return new byte[] { _dataReader.GetByte(ordinal) };
            }
            if (fieldDbType == typeof(Guid))
            {
                return _dataReader.GetGuid(ordinal).ToByteArray();
            }
            if (fieldDbType == typeof(Enum))
            {
                throw new InvalidOperationException(string.Format("Type {0} conversion not supported ", fieldDbType.ToString()));
            }
            if (fieldDbType == typeof(Object))
            {
                throw new InvalidOperationException(string.Format("Type {0} conversion not supported ", fieldDbType.ToString()));
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
            if (fieldDbType == typeof(UInt32))
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
          
            if (fieldDbType == typeof(char))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetChar(ordinal)) };
            }
            if (fieldDbType == typeof(short))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetInt16(ordinal)) };
            }
            if (fieldDbType == typeof(UInt16))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetInt16(ordinal)) };
            }
            if (fieldDbType == typeof(long))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetInt64(ordinal)) };
            }
            if (fieldDbType == typeof(UInt64))
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
        private readonly IDataTypeMetadata[] _fieldTypeMetadatas;
        private readonly DataTypeSystem _dataTypeSystem;

        public QueryDataReader(System.Data.IDataReader dataReader, IReadOnlyDictionary<string, string> columnsMapper, IDataTypeMetadata[] fieldTypeMetadatas, DataTypeSystem dataTypeSystem)
        {
            this._dataReader = dataReader;
            this._columnsMapper = columnsMapper;
            this._fieldTypeMetadatas = fieldTypeMetadatas;
            this._dataTypeSystem = dataTypeSystem;
        }

        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetValueAsInt32(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
            if (fieldDbType == typeof(int))
            {
                return _dataReader.GetInt32(ordinal);
            }
            if (fieldDbType == typeof(UInt32))
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
            if (fieldDbType == typeof(sbyte))
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
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToInt32(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToInt32(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToInt32(_dataReader.GetInt64(ordinal));
            }

            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToInt32(result[0]);
                }
            }
            if (fieldDbType == typeof(TimeSpan))
            {
                return Convert.ToInt32(DateTimeToTimeSpan(_dataReader.GetDateTime(ordinal)));
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        public TimeSpan DateTimeToTimeSpan(DateTime? ts)
        {
            if (!ts.HasValue) return TimeSpan.Zero;
            else return new TimeSpan(0, ts.Value.Hour, ts.Value.Minute, ts.Value.Second, ts.Value.Millisecond);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetValueAsFloat(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
            if (fieldDbType == typeof(bool))
            {
                return (float)Convert.ToSingle(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return _dataReader.GetFloat(ordinal);
            }
            if (fieldDbType == typeof(string))
            {
                return (float)Convert.ToSingle(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return (float)Convert.ToSingle(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return (float)_dataReader.GetDouble(ordinal);
            }
            if (fieldDbType == typeof(int))
            {
                return (float)Convert.ToSingle(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(UInt32))
            {
                return (float)Convert.ToSingle(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return (float)Convert.ToSingle(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return (float)Convert.ToSingle(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(ushort))
            {
                return (float)Convert.ToSingle(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return (float)Convert.ToSingle(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return (float)Convert.ToSingle(_dataReader.GetInt64(ordinal));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double GetValueAsDouble(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
            if (fieldDbType == typeof(bool))
            {
                return (double)Convert.ToDouble(_dataReader.GetBoolean(ordinal));
            }
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
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToDouble(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToDouble(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToDouble(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToDouble(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToDouble(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToDouble(_dataReader.GetInt64(ordinal));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private decimal GetValueAsDecimal(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
            if (fieldDbType == typeof(bool))
            {
                return (decimal)Convert.ToDecimal(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToDecimal(_dataReader.GetChar(ordinal));
            }
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
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToDecimal(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToDecimal(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToDecimal(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToDecimal(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToDecimal(_dataReader.GetInt64(ordinal));
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetValueAsString(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToString(_dataReader.GetBoolean(ordinal));
            }
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
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToString(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToString(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
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
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToString(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToString(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToString(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(Guid))
            {
                return Convert.ToString(_dataReader.GetGuid(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToString(result[0]);
                }
            }
            if (fieldDbType == typeof(TimeSpan))
            {
                return Convert.ToString(_dataReader.GetDateTime(ordinal));
            }

            if (fieldDbType == typeof(DateTimeOffset))
            {
                return Convert.ToString(_dataReader.GetDateTime(ordinal));
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DateTime GetValueAsDateTime(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
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
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToDateTime(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToDateTime(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToDateTime(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToDateTime(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToDateTime(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToDateTime(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToDateTime(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToDateTime(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(DateTimeOffset))
            {
                return _dataReader.GetDateTime(ordinal);
            }
            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToDateTime(result[0]);
                }
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte GetValueAsByte(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToByte(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return _dataReader.GetByte(ordinal);
            }
            if (fieldDbType == typeof(sbyte))
            {
                return _dataReader.GetByte(ordinal);
            }
            if (fieldDbType == typeof(Enum))
            {
                return Convert.ToByte(_dataReader.GetString(ordinal));
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
            if (fieldDbType == typeof(UInt32))
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
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToByte(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToByte(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToByte(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return result[0];
                }

                return default(byte);
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte[] GetValueAsBytes(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
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
            if (fieldDbType == typeof(sbyte))
            {
                return new byte[] { _dataReader.GetByte(ordinal) };
            }
            if (fieldDbType == typeof(Guid))
            {
                return _dataReader.GetGuid(ordinal).ToByteArray();
            }
            if (fieldDbType == typeof(Enum))
            {
                throw new InvalidOperationException(string.Format("Type {0} conversion not supported ", fieldDbType.ToString()));
            }
            if (fieldDbType == typeof(Object))
            {
                throw new InvalidOperationException(string.Format("Type {0} conversion not supported ", fieldDbType.ToString()));
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
            if (fieldDbType == typeof(UInt32))
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

            if (fieldDbType == typeof(char))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetChar(ordinal)) };
            }
            if (fieldDbType == typeof(short))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetInt16(ordinal)) };
            }
            if (fieldDbType == typeof(UInt16))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetInt16(ordinal)) };
            }
            if (fieldDbType == typeof(long))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetInt64(ordinal)) };
            }
            if (fieldDbType == typeof(UInt64))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetInt64(ordinal)) };
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char GetValueAsChar(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);

            if (fieldDbType == typeof(int))
            {
                return Convert.ToChar(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToChar(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToChar(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToChar(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToChar(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToChar(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToChar(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToChar(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToChar(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(DateTime))
            {
                return Convert.ToChar(_dataReader.GetDateTime(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToChar(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToChar(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToChar(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToChar(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToChar(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(Guid))
            {
                return Convert.ToChar(_dataReader.GetGuid(ordinal));
            }

            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToChar(result[0]);
                }
            }
            if (fieldDbType == typeof(TimeSpan))
            {
                return Convert.ToChar(DateTimeToTimeSpan(_dataReader.GetDateTime(ordinal)));
            }
            if (fieldDbType == typeof(Enum))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            if (fieldDbType == typeof(Object))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short GetValueAsShort(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
            if (fieldDbType == typeof(int))
            {
                return Convert.ToInt16(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToInt16(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToInt16(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToInt16(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToInt16(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToInt16(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToInt16(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToInt16(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToInt16(_dataReader.GetByte(ordinal));
            }

            if (fieldDbType == typeof(char))
            {
                return Convert.ToInt16(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToInt16(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToInt16(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToInt16(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToInt16(_dataReader.GetInt64(ordinal));
            }

            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToInt16(result[0]);
                }
            }
            if (fieldDbType == typeof(TimeSpan))
            {
                return Convert.ToInt16(DateTimeToTimeSpan(_dataReader.GetDateTime(ordinal)));
            }
            if (fieldDbType == typeof(Enum))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            if (fieldDbType == typeof(Object))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt16 GetValueAsUnsignedShort(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
            if (fieldDbType == typeof(int))
            {
                return Convert.ToUInt16(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToUInt16(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToUInt16(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToUInt16(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToUInt16(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToUInt16(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToUInt16(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToUInt16(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToUInt16(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToUInt16(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToUInt16(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToUInt16(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToUInt16(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToUInt16(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToUInt16(result[0]);
                }
            }
            if (fieldDbType == typeof(TimeSpan))
            {
                return Convert.ToUInt16(DateTimeToTimeSpan(_dataReader.GetDateTime(ordinal)));
            }
            if (fieldDbType == typeof(Enum))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            if (fieldDbType == typeof(Object))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt32 GetValueAsUnsignedInteger(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
            if (fieldDbType == typeof(int))
            {
                return Convert.ToUInt32(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToUInt32(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToUInt32(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToUInt32(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToUInt32(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToUInt32(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToUInt32(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToUInt32(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToUInt32(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToUInt32(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToUInt32(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToUInt32(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToUInt32(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToUInt32(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToUInt32(result[0]);
                }
            }
            if (fieldDbType == typeof(TimeSpan))
            {
                return Convert.ToUInt32(DateTimeToTimeSpan(_dataReader.GetDateTime(ordinal)));
            }
            if (fieldDbType == typeof(Enum))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            if (fieldDbType == typeof(Object))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetValueAsLong(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
            if (fieldDbType == typeof(int))
            {
                return Convert.ToInt64(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToInt64(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToInt64(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToInt64(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToInt64(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToInt64(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToInt64(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToInt64(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToInt64(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToInt64(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToInt64(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToInt64(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToInt64(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToInt64(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToInt64(result[0]);
                }
            }
            if (fieldDbType == typeof(TimeSpan))
            {
                return Convert.ToInt64(DateTimeToTimeSpan(_dataReader.GetDateTime(ordinal)));
            }
            if (fieldDbType == typeof(Enum))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            if (fieldDbType == typeof(Object))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt64 GetValueAsUnsignedLong(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
            if (fieldDbType == typeof(int))
            {
                return Convert.ToUInt64(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToUInt64(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToUInt64(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToUInt64(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToUInt64(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToUInt64(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToUInt64(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToUInt64(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToUInt64(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToUInt64(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToUInt64(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToUInt64(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToUInt64(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToUInt64(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToUInt64(result[0]);
                }
            }
            if (fieldDbType == typeof(TimeSpan))
            {
                return Convert.ToUInt64(DateTimeToTimeSpan(_dataReader.GetDateTime(ordinal)));
            }
            if (fieldDbType == typeof(Enum))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            if (fieldDbType == typeof(Object))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte GetValueAsSignedByte(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
            if (fieldDbType == typeof(int))
            {
                return Convert.ToSByte(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToSByte(_dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToSByte(_dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToSByte(_dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToSByte(_dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToSByte(_dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToSByte(_dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToSByte(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToSByte(_dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToSByte(_dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToSByte(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToSByte(_dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToSByte(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToSByte(_dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += _dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                if (result.Length > 0)
                {
                    return Convert.ToSByte(result[0]);
                }
            }
            if (fieldDbType == typeof(TimeSpan))
            {
                return Convert.ToSByte(DateTimeToTimeSpan(_dataReader.GetDateTime(ordinal)));
            }
            if (fieldDbType == typeof(Enum))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            if (fieldDbType == typeof(Object))
            {
                throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
           
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TimeSpan GetValueAsTime(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
            if (fieldDbType == typeof(DateTime))
            {
                return DateTimeToTimeSpan(Convert.ToDateTime(_dataReader.GetInt32(ordinal)));
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime GetValueAsDate(int ordinal)
        {
            return GetValueAsDateTime(ordinal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTimeOffset GetValueAsDateTimeOffset(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetValueAsXml(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetValueAsJson(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enum GetValueAsClrEnum(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Object GetValueAsClrType(int ordinal)
        {
            var fieldDbType = _dataReader.GetFieldType(ordinal);
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, _dataReader.GetName(ordinal)));
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




        public char GetValue(Expression<Func<TModel, char>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsChar(columnIndex);
        }

        public char? GetValue(Expression<Func<TModel, char?>> columnExpression)
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
            return GetValueAsChar(columnIndex);
        }


        public short GetValue(Expression<Func<TModel, short>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsShort(columnIndex);
        }


        public short? GetValue(Expression<Func<TModel, short?>> columnExpression)
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
            return GetValueAsShort(columnIndex);
        }


        public UInt16 GetValue(Expression<Func<TModel, UInt16>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsUnsignedShort(columnIndex);
        }

        public UInt16? GetValue(Expression<Func<TModel, UInt16?>> columnExpression)
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
            return GetValueAsUnsignedShort(columnIndex);
        }

        public UInt32 GetValue(Expression<Func<TModel, UInt32>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsUnsignedInteger(columnIndex);
        }

        public UInt32? GetValue(Expression<Func<TModel, UInt32?>> columnExpression)
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
            return GetValueAsUnsignedInteger(columnIndex);
        }

        public long GetValue(Expression<Func<TModel, long>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsLong(columnIndex);
        }

        public long? GetValue(Expression<Func<TModel, long?>> columnExpression)
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
            return GetValueAsLong(columnIndex);
        }

        public UInt64 GetValue(Expression<Func<TModel, UInt64>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsUnsignedLong(columnIndex);
        }

        public UInt64? GetValue(Expression<Func<TModel, UInt64?>> columnExpression)
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
            return GetValueAsUnsignedLong(columnIndex);
        }

        public sbyte GetValue(Expression<Func<TModel, sbyte>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsSignedByte(columnIndex);
        }

        public sbyte? GetValue(Expression<Func<TModel, sbyte?>> columnExpression)
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
            return GetValueAsSignedByte(columnIndex);
        }


        public TimeSpan GetValue(Expression<Func<TModel, TimeSpan>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsTime(columnIndex);
        }

        public TimeSpan? GetValue(Expression<Func<TModel, TimeSpan?>> columnExpression)
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
            return GetValueAsTime(columnIndex);
        }

        public DateTime GetValue(Expression<Func<TModel, DateTime>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsDate(columnIndex);
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
            return GetValueAsDate(columnIndex);
        }

        public DateTimeOffset GetValue(Expression<Func<TModel, DateTimeOffset>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsDateTimeOffset(columnIndex);
        }

        public DateTimeOffset? GetValue(Expression<Func<TModel, DateTimeOffset?>> columnExpression)
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
            return GetValueAsDateTimeOffset(columnIndex);
        }

        public Enum GetValue(Expression<Func<TModel, Enum>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(this._columnsMapper[columnName]);
            return GetValueAsClrEnum(columnIndex);
        }

        private TResult GetInternalValue<TResult, TExprResult>(Expression<Func<TModel, TExprResult>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var ordinal = _dataReader.GetOrdinal(this._columnsMapper[columnName]); if (_dataReader.IsDBNull(ordinal))
            {
                return default(TResult);
            }
            var value = _dataTypeSystem.GetDecoder<TResult>(_fieldTypeMetadatas[ordinal]).DecodeAs(_dataReader, ordinal);
            return value;
        }

        public bool GetValue(Expression<Func<TModel, bool>> columnExpression)
        {
            var result = GetInternalValue<bool?, bool>(columnExpression);
            return result.Value;
        }

        public bool? GetValue(Expression<Func<TModel, bool?>> columnExpression)
        {
            var result = GetInternalValue<bool?, bool?>(columnExpression);
            return result;
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

}
