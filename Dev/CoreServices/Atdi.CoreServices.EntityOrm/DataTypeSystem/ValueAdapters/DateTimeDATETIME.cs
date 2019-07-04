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
    class DateTimeDATETIME : ValueAdapter<DateTime?, DateTime?>
    {
        public DateTimeDATETIME(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override DateTime? DecodeAs(object storeValue)
        {
            if (storeValue == null)
            {
                return (DateTime?)null;
            }
            return Convert.ToDateTime(storeValue);
        }

        public override DateTime? DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            return ValueReader.ReadAsDATETIME(dataReader, ordinal);
        }

        public override DateTime? EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as DateTimeColumnValue).Value;
            if (!source.HasValue)
            {
                return null;
            }
            return source.Value;
        }
    }
    
}
