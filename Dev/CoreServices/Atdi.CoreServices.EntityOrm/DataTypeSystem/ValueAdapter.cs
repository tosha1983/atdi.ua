using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.DataModels;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    public abstract class ValueAdapter<TCodeType, TStoreType> : LoggedObject, IValueTypedEncoder<TStoreType>, IValueTypeDecoder<TCodeType>
    {
        public ValueAdapter(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(logger)
        {
            this.Metadata = dataTypeMetadata;
        }

        public IDataTypeMetadata Metadata { get; }

        public object Decode(object storeValue)
        {
            return this.DecodeAs(storeValue);
        }

        public object Decode(IDataReader dataReader, int ordinal)
        {
            return this.DecodeAs(dataReader, ordinal);
        }

        public abstract TCodeType DecodeAs(object storeValue);
        public abstract TCodeType DecodeAs(IDataReader dataReader, int ordinal);


        public object Encode(ColumnValue columnValue)
        {
            return this.EncodeAs(columnValue);
        }

        public abstract TStoreType EncodeAs(ColumnValue columnValue);
    }
}
