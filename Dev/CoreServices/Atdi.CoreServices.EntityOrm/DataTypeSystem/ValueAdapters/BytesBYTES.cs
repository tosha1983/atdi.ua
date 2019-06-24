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

namespace Atdi.CoreServices.EntityOrm.ValueAdapters
{
    class BytesBYTES : ValueAdapter<byte[], byte[]>
    {
        public BytesBYTES(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override byte[] DecodeAs(object storeValue)
        {
            if (storeValue == null)
            {
                return (byte[])null;
            }
            return (byte[])(storeValue);
        }

        public override byte[] DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            return ValueReader.ReadAsBYTES(dataReader, ordinal);
        }

        public override byte[] EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as BytesColumnValue).Value;
            //if (!source)
            //{
            //    return null;
            //}
            return source;
        }
    }
    
}
