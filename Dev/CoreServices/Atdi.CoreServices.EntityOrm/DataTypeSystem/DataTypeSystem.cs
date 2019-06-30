using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.DataModels;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{

    public sealed class DataTypeSystem : LoggedObject
    {
        private readonly Type[][] _adapterTypes;
        private readonly ConcurrentDictionary<IDataTypeMetadata, IValueAdapter> _adapters;


        public DataTypeSystem(ILogger logger) : base(logger)
        {
            this._adapterTypes = DataTypeSystem.BuildAdapterTypes();
            this._adapters = new ConcurrentDictionary<IDataTypeMetadata, IValueAdapter>();


            this._adapterTypes[(int)DataType.Boolean][(int)DataSourceVarType.BOOL] = typeof(ValueAdapters.BooleanBOOL);
            this._adapterTypes[(int)DataType.Boolean][(int)DataSourceVarType.BIT] = typeof(ValueAdapters.BooleanBIT);
            this._adapterTypes[(int)DataType.Byte][(int)DataSourceVarType.BIT] = typeof(ValueAdapters.ByteBIT);
            this._adapterTypes[(int)DataType.Byte][(int)DataSourceVarType.BYTE] = typeof(ValueAdapters.ByteBYTE);
            this._adapterTypes[(int)DataType.Bytes][(int)DataSourceVarType.BYTES] = typeof(ValueAdapters.BytesBYTES);
            this._adapterTypes[(int)DataType.Bytes][(int)DataSourceVarType.BLOB] = typeof(ValueAdapters.BytesBLOB);
            this._adapterTypes[(int)DataType.String][(int)DataSourceVarType.NVARCHAR] = typeof(ValueAdapters.StringNVARCHAR);
            this._adapterTypes[(int)DataType.String][(int)DataSourceVarType.VARCHAR] = typeof(ValueAdapters.StringVARCHAR);
            this._adapterTypes[(int)DataType.String][(int)DataSourceVarType.NTEXT] = typeof(ValueAdapters.StringNTEXT);
            this._adapterTypes[(int)DataType.String][(int)DataSourceVarType.TEXT] = typeof(ValueAdapters.StringTEXT);
            this._adapterTypes[(int)DataType.Integer][(int)DataSourceVarType.INT32] = typeof(ValueAdapters.IntegerINT32);
            this._adapterTypes[(int)DataType.Long][(int)DataSourceVarType.INT64] = typeof(ValueAdapters.LongINT64);
            this._adapterTypes[(int)DataType.DateTime][(int)DataSourceVarType.DATETIME] = typeof(ValueAdapters.DateTimeDATETIME);
            this._adapterTypes[(int)DataType.DateTimeOffset][(int)DataSourceVarType.DATETIMEOFFSET] = typeof(ValueAdapters.DateTimeOffsetDATETIMEOFFSET);
            this._adapterTypes[(int)DataType.Guid][(int)DataSourceVarType.GUID] = typeof(ValueAdapters.GuidGUID);
            this._adapterTypes[(int)DataType.SignedByte][(int)DataSourceVarType.INT08] = typeof(ValueAdapters.SignedByteINT08);
            this._adapterTypes[(int)DataType.Short][(int)DataSourceVarType.INT16] = typeof(ValueAdapters.ShortINT16);
            this._adapterTypes[(int)DataType.Decimal][(int)DataSourceVarType.DECIMAL] = typeof(ValueAdapters.DecimalDECIMAL);
            this._adapterTypes[(int)DataType.Float][(int)DataSourceVarType.FLOAT] = typeof(ValueAdapters.FloatFLOAT);
            this._adapterTypes[(int)DataType.Double][(int)DataSourceVarType.DOUBLE] = typeof(ValueAdapters.DoubleDOUBLE);

            this._adapterTypes[(int)DataType.String][(int)DataSourceVarType.BLOB] = typeof(ValueAdapters.StringBLOB);
            this._adapterTypes[(int)DataType.String][(int)DataSourceVarType.BYTES] = typeof(ValueAdapters.StringBYTES);

            this._adapterTypes[(int)DataType.ClrType][(int)DataSourceVarType.BLOB] = typeof(ValueAdapters.ClrTypeBLOB);
            this._adapterTypes[(int)DataType.ClrType][(int)DataSourceVarType.BYTES] = typeof(ValueAdapters.ClrTypeBYTES);
        }

        private static Type[][] BuildAdapterTypes()
        {
            var dataTypeCount = Enum.GetNames(typeof(DataType)).Length;
            var sourceVarTypeCount = Enum.GetNames(typeof(DataSourceVarType)).Length;

            var result = new Type[dataTypeCount][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Type[sourceVarTypeCount];
            }

            return result;
        }

        private IValueAdapter CreateValueAdapter(IDataTypeMetadata dataTypeMetadata)
        {
            var adapterType = _adapterTypes[(int)dataTypeMetadata.CodeVarType][(int)dataTypeMetadata.SourceVarType];
            if (adapterType == null)
            {
                throw new InvalidCastException($"Unsupported conversion between the data type '{dataTypeMetadata.CodeVarType}' and the neutral type '{dataTypeMetadata.SourceVarType}'");
            }
            var result = Activator.CreateInstance(adapterType, new object[] { dataTypeMetadata, this.Logger }) as IValueAdapter;
            if (result == null)
            {
                throw new InvalidCastException($"Unsupported conversion between the data type '{dataTypeMetadata.CodeVarType}' and the neutral type '{dataTypeMetadata.SourceVarType}'");
            }
            return result;
        }

        private T GetValueAdapterByDataType<T>(IDataTypeMetadata dataTypeMetadata)
            where T : class, IValueAdapter 
        {
            if (!_adapters.TryGetValue(dataTypeMetadata, out IValueAdapter valueAdapter))
            {
                valueAdapter = this.CreateValueAdapter(dataTypeMetadata);
                if (!_adapters.TryAdd(dataTypeMetadata, valueAdapter))
                {
                    if (!_adapters.TryGetValue(dataTypeMetadata, out valueAdapter))
                    {
                        throw new InvalidOperationException($"Failed to add the value adapter to cache with data type '{dataTypeMetadata}'");
                    }
                }
            }
            var result = valueAdapter as T;
            if (result == null)
            {
                throw new InvalidCastException($"Unsupported conversion between the data type '{dataTypeMetadata.CodeVarType}' and the neutral type '{dataTypeMetadata.SourceVarType}'");
            }

            return result;
        }

        public IValueTypeDecoder<TCodeType> GetDecoder<TCodeType>(IDataTypeMetadata dataTypeMetadata)
        {
            return this.GetValueAdapterByDataType<IValueTypeDecoder<TCodeType>>(dataTypeMetadata);
        }

        public IValueDecoder GetDecoder(IDataTypeMetadata dataTypeMetadata)
        {
            return this.GetValueAdapterByDataType<IValueDecoder>(dataTypeMetadata);
        }

        public IValueTypedEncoder<TStoreType> GetEncoder<TStoreType>(IDataTypeMetadata dataTypeMetadata)
        {
            return this.GetValueAdapterByDataType<IValueTypedEncoder<TStoreType>>(dataTypeMetadata);
        }

        public IValueEncoder GetEncoder(IDataTypeMetadata dataTypeMetadata)
        {
            return this.GetValueAdapterByDataType<IValueEncoder>(dataTypeMetadata);
        }

        public DataType GetSourceDataType(DataSourceVarType sourceVarType)
        {
            switch (sourceVarType)
            {
                case DataSourceVarType.BOOL:
                    return DataType.Boolean;
                case DataSourceVarType.BIT:
                    return DataType.Boolean;
                case DataSourceVarType.BYTE:
                    return DataType.Byte;
                case DataSourceVarType.BYTES:
                    return DataType.Bytes;
                case DataSourceVarType.BLOB:
                    return DataType.Bytes;
                case DataSourceVarType.INT08:
                    return DataType.SignedByte;
                case DataSourceVarType.INT16:
                    return DataType.Short;
                case DataSourceVarType.INT32:
                    return DataType.Integer;
                case DataSourceVarType.INT64:
                    return DataType.Long;
                case DataSourceVarType.NCHAR:
                    return DataType.Char;
                case DataSourceVarType.NCHARS:
                    return DataType.Chars;
                case DataSourceVarType.NVARCHAR:
                    return DataType.String;
                case DataSourceVarType.NTEXT:
                    return DataType.String;
                case DataSourceVarType.CHAR:
                    return DataType.Char;
                case DataSourceVarType.CHARS:
                    return DataType.Chars;
                case DataSourceVarType.VARCHAR:
                    return DataType.String;
                case DataSourceVarType.TEXT:
                    return DataType.String;
                case DataSourceVarType.TIME:
                    return DataType.Time;
                case DataSourceVarType.DATE:
                    return DataType.Date;
                case DataSourceVarType.DATETIME:
                    return DataType.DateTime;
                case DataSourceVarType.DATETIMEOFFSET:
                    return DataType.DateTimeOffset;
                case DataSourceVarType.MONEY:
                    return DataType.Decimal;
                case DataSourceVarType.FLOAT:
                    return DataType.Float;
                case DataSourceVarType.DOUBLE:
                    return DataType.Double;
                case DataSourceVarType.DECIMAL:
                    return DataType.Decimal;
                case DataSourceVarType.GUID:
                    return DataType.Guid;
                case DataSourceVarType.XML:
                    return DataType.Xml;
                case DataSourceVarType.JSON:
                    return DataType.Json;
                case DataSourceVarType.UNDEFINED:
                default:
                    throw new InvalidOperationException($"Unsupported Source Var Type '{sourceVarType}'");
            }
        }
    }
}
