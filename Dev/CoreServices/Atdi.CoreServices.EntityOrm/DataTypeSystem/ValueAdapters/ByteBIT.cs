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
    class ByteBIT : ValueAdapter<byte?, byte?>
    {
        public ByteBIT(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override byte? DecodeAs(object storeValue)
        {
            if (storeValue == null)
            {
                return (byte?)null;
            }
            var valueAsByte = Convert.ToByte(storeValue);
            return valueAsByte;
        }

        public override byte? DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            var valueAsBit = ValueReader.ReadAsBIT(dataReader, ordinal);
            return valueAsBit;
        }

        public override byte? EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as ByteColumnValue).Value;
            if (!source.HasValue)
            {
                return null;
            }
            if (source.Value == 0)
            {
                return (byte)0;
            }
            return (byte)1;
        }
    }
}
