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
    class BooleanBIT : ValueAdapter<bool?, byte?>
    {
        public BooleanBIT(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override bool? DecodeAs(object storeValue)
        {
            if (storeValue == null)
            {
                return (bool?)null;
            }
            var valueAsByte = Convert.ToByte(storeValue);
            return valueAsByte != 0;
        }

        public override bool? DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            var valueAsBit = ValueReader.ReadAsBIT(dataReader, ordinal);
            return valueAsBit != 0;
        }

        public override byte? EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as BooleanColumnValue).Value;
            if (!source.HasValue)
            {
                return null;
            }
            if (source.Value)
            {
                return (byte)1;
            }
            return (byte)0;
        }
    }
}
