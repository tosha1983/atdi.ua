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
    class GuidGUID : ValueAdapter<Guid?, Guid?>
    {
        public GuidGUID(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override Guid? DecodeAs(object storeValue)
        {
            if (storeValue == null)
            {
                return null;
            }
            return (Guid)(storeValue);
        }

        public override Guid? DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            return ValueReader.ReadAsGUID(dataReader, ordinal);
        }

        public override Guid? EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as GuidColumnValue).Value;
            if (!source.HasValue)
            {
                return null;
            }
            return source.Value;
        }
    }
    
}
