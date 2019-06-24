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
    class FloatFLOAT: ValueAdapter<float?, float?>
    {
        public FloatFLOAT(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override float? DecodeAs(object storeValue)
        {
            if (storeValue == null)
            {
                return (float?)null;
            }
            return Convert.ToSingle(storeValue);
        }

        public override float? DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            return ValueReader.ReadAsFLOAT(dataReader, ordinal);
        }

        public override float? EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as FloatColumnValue).Value;
            if (!source.HasValue)
            {
                return null;
            }
            return source.Value;
        }
    }
    
}
