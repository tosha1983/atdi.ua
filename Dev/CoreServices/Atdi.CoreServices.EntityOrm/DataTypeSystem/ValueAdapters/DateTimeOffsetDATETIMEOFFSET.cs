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
    class DateTimeOffsetDATETIMEOFFSET : ValueAdapter<DateTimeOffset?, DateTimeOffset?>
    {
        public DateTimeOffsetDATETIMEOFFSET(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override DateTimeOffset? DecodeAs(object storeValue)
        {
            if (storeValue == null)
            {
                return null;
            }
            return (DateTimeOffset)(storeValue);
        }

        public override DateTimeOffset? DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            return ValueReader.ReadAsDATETIMEOFFSET(dataReader, ordinal);
        }

        public override DateTimeOffset? EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as DateTimeOffsetColumnValue).Value;
            if (!source.HasValue)
            {
                return null;
            }
            return source.Value;
        }
    }
    
}
