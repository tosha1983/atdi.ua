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
    }
}
