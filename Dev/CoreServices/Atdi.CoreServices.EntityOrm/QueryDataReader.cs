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
        private readonly IEngineDataReader _dataReader;
        private readonly IDataTypeMetadata[] _typeMetadatas; 
        private readonly DataTypeSystem _dataTypeSystem;

        public QueryDataReader(IEngineDataReader dataReader, IDataTypeMetadata[] typeMetadatas, DataTypeSystem dataTypeSystem)
        {
            this._dataReader = dataReader;
            this._typeMetadatas = typeMetadatas;
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
                    throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetPath(ordinal)));
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
                    throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetPath(ordinal)));
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
            var value = GetInternalValue<char?>(ordinal);
            return value.Value;
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
            var value = GetInternalValue<short?>(ordinal);
            return value.Value;
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
            var value = GetInternalValue<ushort?>(ordinal);
            return value.Value;
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
            var value = GetInternalValue<uint?>(ordinal);
            return value.Value;
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
            var value = GetInternalValue<long?>(ordinal);
            return value.Value;
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
            var value = GetInternalValue<ulong?>(ordinal);
            return value.Value;
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
            var value = GetInternalValue<sbyte?>(ordinal);
            return value.Value;
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
            var value = GetInternalValue<TimeSpan?>(ordinal);
            return value.Value;
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
            var value = GetInternalValue<DateTime?>(ordinal);
            return value.Value;
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
            var value = GetInternalValue<DateTimeOffset?>(ordinal);
            return value.Value;
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
            var value = GetInternalValue<string>(ordinal);
            return value;
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
            var value = GetInternalValue<string>(ordinal);
            return value;
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
            var value = GetInternalValue<Enum>(ordinal);
            return value;
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
            var value = GetInternalValue<object>(ordinal);
            return value;
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
            var value = GetInternalValue<int?>(ordinal);
            return value.Value;
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
            var value = GetInternalValue<float?>(ordinal);
            return value.Value;
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
            var value = GetInternalValue<double?>(ordinal);
            return value.Value;
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
            var value = GetInternalValue<decimal?>(ordinal);
            return value.Value;
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
            var value = GetInternalValue<string>(ordinal);
            return value;
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
            var value = GetInternalValue<DateTime?>(ordinal);
            return value.Value;
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
            var value = GetInternalValue<byte?>(ordinal);
            return value.Value;
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
            var value = GetInternalValue<Guid?>(ordinal);
            return value.Value;
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
            var value = GetInternalValue<byte[]>(ordinal);
            return value;
        }

        public bool Read()
        {
            return this._dataReader.Read();
        }

        public int GetOrdinal(string path)
        {
            return this._dataReader.GetOrdinalByPath(path);
        }

        public Type GetFieldType(int ordinal)
        {
            return this._dataReader.GetFieldType(ordinal);
        }

        public bool IsDBNull(int ordinal)
        {
            return this._dataReader.IsDBNull(ordinal);
        }

        private T GetInternalValue<T>(int ordinal)
        {
            var typeMetadata = _typeMetadatas[ordinal];
            return _dataTypeSystem.GetDecoder<T>(typeMetadata).DecodeAs(_dataReader, ordinal);
        }
    }

    internal sealed class QueryDataReader<TModel> : IDataReader<TModel>
    {
        private readonly IEngineDataReader _dataReader;
        private readonly IDataTypeMetadata[] _typeMetadatas;
        private readonly DataTypeSystem _dataTypeSystem;

        public QueryDataReader(IEngineDataReader dataReader, IDataTypeMetadata[] typeMetadatas, DataTypeSystem dataTypeSystem)
        {
            this._dataReader = dataReader;
            this._typeMetadatas = typeMetadatas;
            this._dataTypeSystem = dataTypeSystem;
        }

        private static TimeSpan DateTimeToTimeSpan(DateTime? ts)
        {
            if (!ts.HasValue) return TimeSpan.Zero;
            else return new TimeSpan(0, ts.Value.Hour, ts.Value.Minute, ts.Value.Second, ts.Value.Millisecond);
        }


        #region GetValue

        public char GetValue(Expression<Func<TModel, char>> columnExpression)
        {
            var result = GetInternalValue<char?, char>(columnExpression);
            return result.Value;
        }

        public char? GetValue(Expression<Func<TModel, char?>> columnExpression)
        {
            var result = GetInternalValue<char?, char?>(columnExpression);
            return result;
        }

        public short GetValue(Expression<Func<TModel, short>> columnExpression)
        {
            var result = GetInternalValue<short?, short>(columnExpression);
            return result.Value;
        }

        public short? GetValue(Expression<Func<TModel, short?>> columnExpression)
        {
            var result = GetInternalValue<short?, short?>(columnExpression);
            return result;
        }

        public UInt16 GetValue(Expression<Func<TModel, UInt16>> columnExpression)
        {
            var result = GetInternalValue<ushort?, ushort>(columnExpression);
            return result.Value;
        }

        public UInt16? GetValue(Expression<Func<TModel, UInt16?>> columnExpression)
        {
            var result = GetInternalValue<ushort?, ushort?>(columnExpression);
            return result;
        }

        public UInt32 GetValue(Expression<Func<TModel, UInt32>> columnExpression)
        {
            var result = GetInternalValue<uint?, uint>(columnExpression);
            return result.Value;
        }

        public UInt32? GetValue(Expression<Func<TModel, UInt32?>> columnExpression)
        {
            var result = GetInternalValue<uint?, uint?>(columnExpression);
            return result;
        }

        public long GetValue(Expression<Func<TModel, long>> columnExpression)
        {
            var result = GetInternalValue<long?, long>(columnExpression);
            return result.Value;
        }

        public long? GetValue(Expression<Func<TModel, long?>> columnExpression)
        {
            var result = GetInternalValue<long?, long?>(columnExpression);
            return result;
        }

        public UInt64 GetValue(Expression<Func<TModel, UInt64>> columnExpression)
        {
            var result = GetInternalValue<ulong?, ulong>(columnExpression);
            return result.Value;
        }

        public UInt64? GetValue(Expression<Func<TModel, UInt64?>> columnExpression)
        {
            var result = GetInternalValue<ulong?, ulong?>(columnExpression);
            return result;
        }

        public sbyte GetValue(Expression<Func<TModel, sbyte>> columnExpression)
        {
            var result = GetInternalValue<sbyte?, sbyte>(columnExpression);
            return result.Value;
        }

        public sbyte? GetValue(Expression<Func<TModel, sbyte?>> columnExpression)
        {
            var result = GetInternalValue<sbyte?, sbyte?>(columnExpression);
            return result;
        }
        
        public TimeSpan GetValue(Expression<Func<TModel, TimeSpan>> columnExpression)
        {
            var result = GetInternalValue<TimeSpan?, TimeSpan>(columnExpression);
            return result.Value;
        }

        public TimeSpan? GetValue(Expression<Func<TModel, TimeSpan?>> columnExpression)
        {
            var result = GetInternalValue<TimeSpan?, TimeSpan?>(columnExpression);
            return result;
        }

        public DateTime GetValue(Expression<Func<TModel, DateTime>> columnExpression)
        {
            var result = GetInternalValue<DateTime?, DateTime>(columnExpression);
            return result.Value;
        }

        public DateTime? GetValue(Expression<Func<TModel, DateTime?>> columnExpression)
        {
            var result = GetInternalValue<DateTime?, DateTime?>(columnExpression);
            return result;
        }

        public DateTimeOffset GetValue(Expression<Func<TModel, DateTimeOffset>> columnExpression)
        {
            var result = GetInternalValue<DateTimeOffset?, DateTimeOffset>(columnExpression);
            return result.Value;
        }

        public DateTimeOffset? GetValue(Expression<Func<TModel, DateTimeOffset?>> columnExpression)
        {
            var result = GetInternalValue<DateTimeOffset?, DateTimeOffset?>(columnExpression);
            return result;
        }

        public Enum GetValue(Expression<Func<TModel, Enum>> columnExpression)
        {
            var result = GetInternalValue<Enum, Enum>(columnExpression);
            return result;
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
            var result = GetInternalValue<int?, int>(columnExpression);
            return result.Value;
        }

        public int? GetValue(Expression<Func<TModel, int?>> columnExpression)
        {
            var result = GetInternalValue<int?, int?>(columnExpression);
            return result;
        }

        public float GetValue(Expression<Func<TModel, float>> columnExpression)
        {
            var result = GetInternalValue<float?, float>(columnExpression);
            return result.Value;
        }

        public float? GetValue(Expression<Func<TModel, float?>> columnExpression)
        {
            var result = GetInternalValue<float?, float?>(columnExpression);
            return result;
        }

        public double GetValue(Expression<Func<TModel, double>> columnExpression)
        {
            var result = GetInternalValue<double?, double>(columnExpression);
            return result.Value;
        }

        public double? GetValue(Expression<Func<TModel, double?>> columnExpression)
        {
            var result = GetInternalValue<double?, double?>(columnExpression);
            return result;
        }

        public decimal GetValue(Expression<Func<TModel, decimal>> columnExpression)
        {
            var result = GetInternalValue<decimal?, decimal>(columnExpression);
            return result.Value;
        }

        public decimal? GetValue(Expression<Func<TModel, decimal?>> columnExpression)
        {
            var result = GetInternalValue<decimal?, decimal?>(columnExpression);
            return result;
        }

        public string GetValue(Expression<Func<TModel, string>> columnExpression)
        {
            var result = GetInternalValue<string, string>(columnExpression);
            return result;
        }

        public byte GetValue(Expression<Func<TModel, byte>> columnExpression)
        {
            var result = GetInternalValue<byte?, byte>(columnExpression);
            return result.Value;
        }

        public byte? GetValue(Expression<Func<TModel, byte?>> columnExpression)
        {
            var result = GetInternalValue<byte?, byte?>(columnExpression);
            return result;
        }

        public Guid GetValue(Expression<Func<TModel, Guid>> columnExpression)
        {
            var result = GetInternalValue<Guid?, Guid>(columnExpression);
            return result.Value;
        }

        public Guid? GetValue(Expression<Func<TModel, Guid?>> columnExpression)
        {
            var result = GetInternalValue<Guid?, Guid?>(columnExpression);
            return result;
        }

        public byte[] GetValue(Expression<Func<TModel, byte[]>> columnExpression)
        {
            var result = GetInternalValue<byte[], byte[]>(columnExpression);
            return result;
        }

        #endregion

        #region GetValue[]

        public int[] GetValue(Expression<Func<TModel, int[]>> columnExpression)
        {
            var result = GetInternalValue<int[], int[]>(columnExpression);
            return result;
        }

        public float[] GetValue(Expression<Func<TModel, float[]>> columnExpression)
        {
            var result = GetInternalValue<float[], float[]>(columnExpression);
            return result;
        }

        public double[] GetValue(Expression<Func<TModel, double[]>> columnExpression)
        {
            var result = GetInternalValue<double[], double[]>(columnExpression);
            return result;
        }

        public decimal[] GetValue(Expression<Func<TModel, decimal[]>> columnExpression)
        {
            var result = GetInternalValue<decimal[], decimal[]>(columnExpression);
            return result;
        }

        public bool[] GetValue(Expression<Func<TModel, bool[]>> columnExpression)
        {
            var result = GetInternalValue<bool[], bool[]>(columnExpression);
            return result;
        }

        public string[] GetValue(Expression<Func<TModel, string[]>> columnExpression)
        {
            var result = GetInternalValue<string[], string[]>(columnExpression);
            return result;
        }

        public DateTime[] GetValue(Expression<Func<TModel, DateTime[]>> columnExpression)
        {
            var result = GetInternalValue<DateTime[], DateTime[]>(columnExpression);
            return result;
        }

        public Guid[] GetValue(Expression<Func<TModel, Guid[]>> columnExpression)
        {
            var result = GetInternalValue<Guid[], Guid[]>(columnExpression);
            return result;
        }

        public char[] GetValue(Expression<Func<TModel, char[]>> columnExpression)
        {
            var result = GetInternalValue<char[], char[]>(columnExpression);
            return result;
        }

        public short[] GetValue(Expression<Func<TModel, short[]>> columnExpression)
        {
            var result = GetInternalValue<short[], short[]>(columnExpression);
            return result;
        }

        public ushort[] GetValue(Expression<Func<TModel, ushort[]>> columnExpression)
        {
            var result = GetInternalValue<ushort[], ushort[]>(columnExpression);
            return result;
        }

        public uint[] GetValue(Expression<Func<TModel, uint[]>> columnExpression)
        {
            var result = GetInternalValue<uint[], uint[]>(columnExpression);
            return result;
        }

        public long[] GetValue(Expression<Func<TModel, long[]>> columnExpression)
        {
            var result = GetInternalValue<long[], long[]>(columnExpression);
            return result;
        }

        public ulong[] GetValue(Expression<Func<TModel, ulong[]>> columnExpression)
        {
            var result = GetInternalValue<ulong[], ulong[]>(columnExpression);
            return result;
        }

        public sbyte[] GetValue(Expression<Func<TModel, sbyte[]>> columnExpression)
        {
            var result = GetInternalValue<sbyte[], sbyte[]>(columnExpression);
            return result;
        }

        public TimeSpan[] GetValue(Expression<Func<TModel, TimeSpan[]>> columnExpression)
        {
            var result = GetInternalValue<TimeSpan[], TimeSpan[]>(columnExpression);
            return result;
        }

        public DateTimeOffset[] GetValue(Expression<Func<TModel, DateTimeOffset[]>> columnExpression)
        {
            var result = GetInternalValue<DateTimeOffset[], DateTimeOffset[]>(columnExpression);
            return result;
        }



		#endregion

		public bool IsNotNull(Expression<Func<TModel, object>> columnExpression)
		{
			if (columnExpression == null)
			{
				throw new ArgumentNullException(nameof(columnExpression));
			}

			var fieldPath = columnExpression.Body.GetMemberName();
			var ordinal = _dataReader.GetOrdinalByPath(fieldPath);
			return !_dataReader.IsDBNull(ordinal);
		}

		public bool IsNull(Expression<Func<TModel, object>> columnExpression)
		{
			if (columnExpression == null)
			{
				throw new ArgumentNullException(nameof(columnExpression));
			}

			var fieldPath = columnExpression.Body.GetMemberName();
			var ordinal = _dataReader.GetOrdinalByPath(fieldPath);
			return _dataReader.IsDBNull(ordinal);
		}

		public bool Read()
        {
            return this._dataReader.Read();
        }

        private TResult GetInternalValue<TResult, TExprResult>(Expression<Func<TModel, TExprResult>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var fieldPath = columnExpression.Body.GetMemberName();
            var ordinal = _dataReader.GetOrdinalByPath(fieldPath);
            if (_dataReader.IsDBNull(ordinal))
            {
                return default(TResult);
            }
            var dataType = _typeMetadatas[ordinal];
            if (dataType.CodeVarType != DataType.ClrType)
            {
                var value = _dataTypeSystem.GetDecoder<TResult>(dataType).DecodeAs(_dataReader, ordinal);
                return value;
            }

            var clrValue = _dataTypeSystem.GetDecoder<object>(dataType).DecodeAs(_dataReader, ordinal);
            return (TResult)clrValue;

        }

        
    }

}
