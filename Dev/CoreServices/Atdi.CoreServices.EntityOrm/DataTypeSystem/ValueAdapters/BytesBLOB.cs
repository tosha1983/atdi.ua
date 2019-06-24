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
    class BytesBLOB : ValueAdapter<byte[], byte[]>
    {
        public BytesBLOB(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
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
            return ValueReader.ReadAsBLOB(dataReader, ordinal);
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
