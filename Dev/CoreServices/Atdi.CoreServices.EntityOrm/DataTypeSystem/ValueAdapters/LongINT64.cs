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
    class LongINT64: ValueAdapter<long?, long?>
    {
        public LongINT64(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override long? DecodeAs(object storeValue)
        {
            if (storeValue == null)
            {
                return (long?)null;
            }
            return Convert.ToInt64(storeValue);
        }

        public override long? DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            return ValueReader.ReadAsINT64(dataReader, ordinal);
        }

        public override long? EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as LongColumnValue).Value;
            if (!source.HasValue)
            {
                return null;
            }
            return source.Value;
        }
    }
    
}
