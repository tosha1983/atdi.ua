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
    class IntegerINT32 : ValueAdapter<int?, int?>
    {
        public IntegerINT32(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override int? DecodeAs(object storeValue)
        {
            if (storeValue == null)
            {
                return (int?)null;
            }
            return Convert.ToInt32(storeValue);
        }

        public override int? DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            return ValueReader.ReadAsINT32(dataReader, ordinal);
        }

        public override int? EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as IntegerColumnValue).Value;
            if (!source.HasValue)
            {
                return null;
            }
            return source.Value;
        }
    }
    
}
