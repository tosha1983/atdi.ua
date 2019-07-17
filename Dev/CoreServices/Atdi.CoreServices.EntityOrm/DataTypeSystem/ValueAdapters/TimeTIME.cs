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
    class TimeTIME : ValueAdapter<TimeSpan?, TimeSpan?>
    {
        public TimeTIME(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override TimeSpan? DecodeAs(object storeValue)
        {
            if (storeValue == null)
            {
                return (TimeSpan?)null;
            }
            return (TimeSpan?)storeValue;
        }

        public override TimeSpan? DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            return ValueReader.ReadAsTIME(dataReader, ordinal);
        }

        public override TimeSpan? EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as TimeColumnValue).Value;
            if (!source.HasValue)
            {
                return null;
            }
            return source.Value;
        }
    }
    
}
