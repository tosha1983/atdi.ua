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
    class DecimalDECIMAL: ValueAdapter<decimal?, decimal?>
    {
        public DecimalDECIMAL(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override decimal? DecodeAs(object storeValue)
        {
            if (storeValue == null)
            {
                return (decimal?)null;
            }
            return Convert.ToDecimal(storeValue);
        }

        public override decimal? DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            return ValueReader.ReadAsDECIMAL(dataReader, ordinal);
        }

        public override decimal? EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as DecimalColumnValue).Value;
            if (!source.HasValue)
            {
                return null;
            }
            return source.Value;
        }
    }
    
}
