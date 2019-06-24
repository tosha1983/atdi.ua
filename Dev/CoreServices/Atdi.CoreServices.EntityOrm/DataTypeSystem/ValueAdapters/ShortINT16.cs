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
    class ShortINT16 : ValueAdapter<short?, short?>
    {
        public ShortINT16(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override short? DecodeAs(object storeValue)
        {
            if (storeValue == null)
            {
                return (short?)null;
            }
            return Convert.ToInt16(storeValue);
        }

        public override short? DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            return ValueReader.ReadAsINT16(dataReader, ordinal);
        }

        public override short? EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as ShortColumnValue).Value;
            if (!source.HasValue)
            {
                return null;
            }
            return source.Value;
        }
    }
    
}
