using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.DataModels;
using Atdi.Platform.Logging;
using Atdi.Common.Extensions;
namespace Atdi.CoreServices.EntityOrm.ValueAdapters
{
    class ClrTypeBYTES : ValueAdapter<object, byte[]>
    {
        public ClrTypeBYTES(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override object  DecodeAs(object storeValue)
        {
            if (storeValue == null)
            {
                return (string)null;
            }
            var store = (byte[])storeValue;
            var value = store.Deserialize<object>();
            return value;
        }

        public override object DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            var store = ValueReader.ReadAsBYTES(dataReader, ordinal);
            var value = store.Deserialize<object>();
            return value;
        }

        public override byte[] EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as ClrTypeColumnValue).Value;
            if (source == null)
            {
                return null;
            }
            return source.Serialize();
        }
    }
    
}
