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
    class DoubleDOUBLE: ValueAdapter<double?, double?>
    {
        public DoubleDOUBLE(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override double? DecodeAs(object storeValue)
        {
            if (storeValue == null)
            {
                return null;
            }
            return Convert.ToDouble(storeValue);
        }

        public override double? DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            return ValueReader.ReadAsDOUBLE(dataReader, ordinal);
        }

        public override double? EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as DoubleColumnValue).Value;
            if (!source.HasValue)
            {
                return null;
            }
            return source.Value;
        }
    }
    
}
